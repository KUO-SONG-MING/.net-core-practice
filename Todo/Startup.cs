using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Interface;
using Todo.Models;
using Todo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Todo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Scaffold-DbContext "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\Desktop\.NET core\Todo.mdf;Integrated Security=True;Connect Timeout=30" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force -CoNtext TodoContext
            services.AddControllers().AddNewtonsoftJson();
            services.AddDbContext<TodoContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("TodoDatabase")));
            //model自動映射
            services.AddAutoMapper(typeof(Startup));
            //手動注入DI，一次Request只會產生一個注入實體
            services.AddScoped<TodoListService>();
            services.AddScoped<AsyncService>();

            //手動注入DI，程式啟動時就會產生唯一一次實體，所有class都注入此實體直到程式運行中止
            //services.AddSingleton<TodoListService>();

            //手動注入DI，一次DI注入就會產生一個注入實體(例:不同class注入同一個class就會產生各自的實體)
            //services.AddTransient<TodoListService>();

            //手動注入DI，並使用IOC的方式在此控制要注入哪一個實體
            services.AddScoped<ITodoList, TodoListServiceTest1>();
            services.AddScoped<ITodoList, TodoListServiceTest2>();

            //注入HttpContext
            services.AddHttpContextAccessor();

            ////加入以cookie方式驗證的機制，未登入時導向以下路由
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
            //{
            //    //沒有登入cookie時導向以下
            //    option.LoginPath = new PathString("/api/Login/NoLogin");
            //    //cookie裡面的Role設定不對時導向以下
            //    option.AccessDeniedPath = new PathString("/api/Login/NoAccess");
            //    //5分鐘之後自動登出(使cookie失效)
            //    option.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            //});

            //加入以Jwt驗證是否登入
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //是否驗證發行者
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    //是否驗證Audience
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    //是否驗證過期時效
                    ValidateLifetime = true,
                    //驗證金鑰
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:KEY"]))
                };
            });

            ////設定全部controler都需要檢查授權驗證，也就是AOP的概念
            //services.AddMvc(option => 
            //{
            //    option.Filters.Add(new AuthorizeFilter);
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
