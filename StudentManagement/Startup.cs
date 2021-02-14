using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCore.AutoRegisterDi;
using StudentManagement.CustomerMiddlewares;
using StudentManagement.Infrastructure;
using StudentManagement.Infrastructure.Data;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Models;
using StudentManagement.Security;
using System;

namespace StudentManagement
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            this._env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddControllersWithViews(config =>
             {
                 var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                 config.Filters.Add(new AuthorizeFilter(policy));
             });
            if (_env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_configuration.GetConnectionString("StudentDbConnection"))
            );
            services.AddIdentity<ApplicationUser, IdentityRole>().AddErrorDescriber<CustomerIdentityErrorDescriber>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                // �����û�����¼�����ԡ������
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = true;
                // �����˻�����
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            });
            services.AddAuthorization(options =>
            {
                // ���Խ��������Ȩ
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));
                // ���Խ�Ͻ�ɫ��Ȩ
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin", "User"));
                options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context => AuthorizeAccess(context)));
            });
            services.ConfigureApplicationCookie(options =>
            {
                // ���þܾ����ʵ�·�ɵ�ַ
                options.AccessDeniedPath = new PathString("/Admin/AccessDenied");
                // �޸ĵ�¼��·�ɵ�ַ
                //options.LoginPath = new PathString("/Admin/Login");
                //�޸�ע����·�ɵ�ַ
                //options.LogoutPath = new PathString("/Admin/Logout");
                // ͳһϵͳ��ȫ��Cookie����
                options.Cookie.Name = "StudentManagement";
                // ��¼�û�cookie����Ч��
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                // �Ƿ��cookie���û�������ʱ��
                options.SlidingExpiration = true;
            });
            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = _configuration["Authentication:Microsoft:ClientId"]; //��ȡappsetting.json�ļ��е�����
                microsoftOptions.ClientSecret = _configuration["Authentication:Microsoft:ClientSecret"];
            });
            services.AddSingleton<DataProtectionPurposeStrings>();
            services.AddTransient(typeof(IRepository<,>), typeof(RepositoryBase<,>));
            // ʹ��AutoRegisterDiʵ���Զ�ע����Service��β���ļ�
            services.RegisterAssemblyPublicNonGenericClasses().Where(c => c.Name.EndsWith("Service")).AsPublicImplementedInterfaces(ServiceLifetime.Scoped);
            //services.AddScoped<IStudentService, StudentService>();
            //services.AddScoped<ICourseService, CourseService>();
            // ע��HttpContextAccessor
            //services.AddHttpContextAccessor();
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
            //});
            //services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            //services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDataInitializer(); // �������ݳ�ʼ������
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseStaticFiles();
            app.UseAuthentication(); // ����û���֤�м��
            //app.UseMvcWithDefaultRoute();
            app.UseRouting();
            app.UseAuthorization(); //�����Ȩ�м��
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // ��Ȩ����
        private bool AuthorizeAccess(AuthorizationHandlerContext context)
        {
            return context.User.IsInRole("Admin") &&
                context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                context.User.IsInRole("Super Admin");
        }
    }
}