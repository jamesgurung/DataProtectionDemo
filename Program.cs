using System.Runtime.InteropServices;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

var dpBuilder = builder.Services.AddDataProtection()
  .PersistKeysToAzureBlobStorage(new Uri(builder.Configuration["DataProtectionBlobUri"]));

// In .NET 6 RTM, manually set the Application Name to the RC2 default for backwards compatibility
if (RuntimeInformation.FrameworkDescription.Contains("rtm")) {
  var legacyContentRootPath = builder.Environment.ContentRootPath.TrimEnd('/');
  dpBuilder.SetApplicationName(legacyContentRootPath);
}

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.MapGet("/", (IDataProtectionProvider dataProtectionProvider, IWebHostEnvironment env) =>
{
  var protector = dataProtectionProvider.CreateProtector("test");
  return $"Running {RuntimeInformation.FrameworkDescription}\n\n" +
    $"Encrypted message: {protector.Protect("This is a test.")}";
});

app.MapGet("/{encryptedText}", (string encryptedText, IDataProtectionProvider dataProtectionProvider, IWebHostEnvironment env) =>
{
  var protector = dataProtectionProvider.CreateProtector("test");
  return $"Running {RuntimeInformation.FrameworkDescription}\n\n" +
    $"Decrypted message: {protector.Unprotect(encryptedText)}";
});

app.Run();
