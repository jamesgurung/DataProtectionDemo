using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Environment.ContentRootPath = builder.Environment.ContentRootPath.TrimEnd('/');

builder.Services.AddDataProtection()
  .PersistKeysToAzureBlobStorage(new Uri(config["DataProtectionBlobUri"]))
  .SetApplicationName("shared app name");

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.MapGet("/", (IDataProtectionProvider dataProtectionProvider, IWebHostEnvironment env) =>
{
  var protector = dataProtectionProvider.CreateProtector("test");
  return $"Running {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}\n" +
    $"ContentRoot: {env.ContentRootPath}\n\n" +
    $"Encrypted message: {protector.Protect("This is a test.")}";
});

app.MapGet("/{encryptedText}", (string encryptedText, IDataProtectionProvider dataProtectionProvider, IWebHostEnvironment env) =>
{
  var protector = dataProtectionProvider.CreateProtector("test");
  return $"Running {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}\n" +
    $"ContentRoot: {env.ContentRootPath}\n\n" +
    $"Decrypted message: {protector.Unprotect(encryptedText)}";
});

app.Run();
