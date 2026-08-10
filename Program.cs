using st10157545_abcretail.Services;


var builder = WebApplication.CreateBuilder(args);

// MVC services
builder.Services.AddControllersWithViews();

// Register our Azure Storage services for dependency injection.
// Both services read the connection string from configuration (appsettings.json
// or, better, from environment variables / user-secrets so the real key never
// gets committed to source control).
builder.Services.AddSingleton<ITableStorageService, TableStorageService>();
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
builder.Services.AddSingleton<IQueueStorageService, QueueStorageService>();
builder.Services.AddSingleton<IFileStorageService, FileStorageService>();

var app = builder.Build();

// Make sure the Azure Table Storage tables and Blob Storage container exist
// before the app starts serving requests.
using (var scope = app.Services.CreateScope())
{
    var tableService = scope.ServiceProvider.GetRequiredService<ITableStorageService>();
    var blobService = scope.ServiceProvider.GetRequiredService<IBlobStorageService>();
    var queueService = scope.ServiceProvider.GetRequiredService<IQueueStorageService>();
    var fileService = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
    await tableService.InitializeAsync();
    await blobService.InitializeAsync();
    await queueService.InitializeAsync();
    await fileService.InitializeAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
