using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddDataProtection()
  .PersistKeysToAzureBlobStorage(new Uri(config["DataProtectionBlobUri"]));

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.MapGet("/", (IDataProtectionProvider dataProtectionProvider) =>
{
  var protector = dataProtectionProvider.CreateProtector("test");
  return $"Running {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}\n\n" +
    $"Encrypted message: {protector.Protect("This is a test.")}";
});

app.MapGet("/{encryptedText}", (string encryptedText, IDataProtectionProvider dataProtectionProvider) =>
{
  var protector = dataProtectionProvider.CreateProtector("test");
  return $"Running {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}\n\n" +
    $"Decrypted message: {protector.Unprotect(encryptedText)}";
});

app.Run();
