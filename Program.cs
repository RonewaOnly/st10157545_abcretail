using st10157545_abcretail.Data;
using st10157545_abcretail.Services;
using Microsoft.EntityFrameworkCore;


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

//

var app = builder.Build();

// Make sure the Azure Table Storage tables and Blob Storage container exist
// before the app starts serving requests.
using (var scope = app.Services.CreateScope())
{
    var tableService = scope.ServiceProvider.GetRequiredService<ITableStorageService>();
    var blobService = scope.ServiceProvider.GetRequiredService<IBlobStorageService>();
    var queueService = scope.ServiceProvider.GetRequiredService<IQueueStorageService>();
    var fileService = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
    try
    {
        Console.WriteLine("Initializing Table Storage...");
        await tableService.InitializeAsync();
        Console.WriteLine("Table Storage OK");
    }
    catch (Exception ex)
    {
        Console.WriteLine("TABLE STORAGE FAILED:");
        Console.WriteLine(ex);
        throw;
    }

    try
    {
        Console.WriteLine("Initializing Blob Storage...");
        await blobService.InitializeAsync();
        Console.WriteLine("Blob Storage OK");
    }
    catch (Exception ex)
    {
        Console.WriteLine("BLOB STORAGE FAILED:");
        Console.WriteLine(ex);
        throw;
    }

    try
    {
        Console.WriteLine("Initializing Queue Storage...");
        await queueService.InitializeAsync();
        Console.WriteLine("Queue Storage OK");
    }
    catch (Exception ex)
    {
        Console.WriteLine("QUEUE STORAGE FAILED:");
        Console.WriteLine(ex);
        throw;
    }

    try
    {
        Console.WriteLine("Initializing File Storage...");
        await fileService.InitializeAsync();
        Console.WriteLine("File Storage OK");
    }
    catch (Exception ex)
    {
        Console.WriteLine("FILE STORAGE FAILED:");
        Console.WriteLine(ex);
        throw;
    }
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
