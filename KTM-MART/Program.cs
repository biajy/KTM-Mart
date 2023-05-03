using KTM_MART.DataAccess;
using KTM_MART.DataAccess.Repository;
using KTM_MART.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using KTM_MART.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using KTM_MART.Utility;
using Stripe;
using KTM_MART.Service.Apriori;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders() 
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IAprioriService>(provider =>
{
    TrainingSet set = new TrainingSet("Customer", "Product");
    //var set = provider.GetService<TrainingSet>();
    set.AddSample(new TrainingSample(1, new List<string>() { "Current Noodle", "Amul kool", "Coca-Cola", "Salt", "Mozzarella Cheese", "Egg", "Dishwasher" }));
    set.AddSample(new TrainingSample(2, new List<string>() { "Beer", "Beef", "Coca-cola", "BBQ Sauce", "Garlic", "Ginger", "Wine", "Chicken", "Salt", "Oil", "Tomato", "Garbage Bag", "Dishwasher" }));
    set.AddSample(new TrainingSample(3, new List<string>() { "Egg", "Milk", "Brown Bread", "Butter", "Avacado", "Khujurico Puff", "Jam" }));
    set.AddSample(new TrainingSample(4, new List<string>() { "Cheese Cracker", "Milk", "Oreo", "Apple", "Banana", "Khajurico Puff" }));
   
    set.AddSample(new TrainingSample(5, new List<string>() { "WaiWai", "Tomato", "Ginger", "Garlic", "Icecream", "Oil", "Oreo", "Lemon","Egg", "Salt" }));
   
    set.AddSample(new TrainingSample(6, new List<string>() { "WaterMelon", "Icecream", "Milk", "Brown Bread", "Frooti", "Chocolate", "Oreo", "Bread", "jam" }));
    set.AddSample(new TrainingSample(7, new List<string>() { " Brown bread", "Yogurt", "Egg", "Butter", "Apple", "Banana", "Orange" }));
    set.AddSample(new TrainingSample(8, new List<string>() { "Amul Kool", "Cookies", "Current Noodle", "Coca-Cola", "Egg", "Dishwasher" }));
    set.AddSample(new TrainingSample(9, new List<string>() { "Mushroom", "Salt", "Oil", "Chicken", "BBQ Sauce", "Potato", "Tomato", "Wine", "Garbage bag" }));
    set.AddSample(new TrainingSample(10, new List<string>() { "Brown bread", "Milk", "Egg", "Butter", "Avocado", "Apple", "Banana", "Jam" }));
    
   
   
    set.Lock();
    return new ApriroiService(set, 60, 100);
});
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "161520509717118";
    options.AppSecret = "b66a3a0b1faff22663ebf7e4e7342e0b";
});
builder.Services.ConfigureApplicationCookie(options =>
{
	//// Cookie settings  
	//options.Cookie.HttpOnly = true;
	//options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    // We have to over write this path because the default provided path for login/logout
    // "/Account/Login" so we have to over write the predefined path as 
    // "/Identity/Account/Login"
	options.LoginPath = "/Identity/Account/Login";     //set the login path.  
    options.LogoutPath = "/Identity/Account/Logout";  //set the logout path
	options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // set the accessdeniedpath
	options.SlidingExpiration = true;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseAuthentication();;

app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
