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
            //model�۰ʬM�g
            services.AddAutoMapper(typeof(Startup));
            //��ʪ`�JDI�A�@��Request�u�|���ͤ@�Ӫ`�J����
            services.AddScoped<TodoListService>();
            services.AddScoped<AsyncService>();

            //��ʪ`�JDI�A�{���ҰʮɴN�|���Ͱߤ@�@������A�Ҧ�class���`�J�����骽��{���B�椤��
            //services.AddSingleton<TodoListService>();

            //��ʪ`�JDI�A�@��DI�`�J�N�|���ͤ@�Ӫ`�J����(��:���Pclass�`�J�P�@��class�N�|���ͦU�۪�����)
            //services.AddTransient<TodoListService>();

            //��ʪ`�JDI�A�èϥ�IOC���覡�b������n�`�J���@�ӹ���
            services.AddScoped<ITodoList, TodoListServiceTest1>();
            services.AddScoped<ITodoList, TodoListServiceTest2>();

            //�`�JHttpContext
            services.AddHttpContextAccessor();

            ////�[�J�Hcookie�覡���Ҫ�����A���n�J�ɾɦV�H�U����
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
            //{
            //    //�S���n�Jcookie�ɾɦV�H�U
            //    option.LoginPath = new PathString("/api/Login/NoLogin");
            //    //cookie�̭���Role�]�w����ɾɦV�H�U
            //    option.AccessDeniedPath = new PathString("/api/Login/NoAccess");
            //    //5��������۰ʵn�X(��cookie����)
            //    option.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            //});

            //�[�J�HJwt���ҬO�_�n�J
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //�O�_���ҵo���
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    //�O�_����Audience
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    //�O�_���ҹL���ɮ�
                    ValidateLifetime = true,
                    //���Ҫ��_
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:KEY"]))
                };
            });

            ////�]�w����controler���ݭn�ˬd���v���ҡA�]�N�OAOP������
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
