using System;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using FunctionApp5.Data;
using FunctionApp5.Repository;
using FunctionApp5.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;

namespace FunctionApp5
{
    public class Function1
    {
        
       
        [FunctionName("Function1")]
        public async Task RunAsync([BlobTrigger("files-container/{name}.{extension}", Connection = "BlobConnectionString")] Stream myBlob, Uri uri, string name, string extension, ILogger log)
        {
            try
            {
                log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes\nconstr: {Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection")} ");

                var _emailSender = new EmailSender();

                var str = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");
                using (var _applicationContext = new ApplicationContext(str))
                {
                    var obj = _applicationContext.DbModelDatas.FirstOrDefaultAsync(d => d.FileName == name + "." + extension).Result;

                    if (obj != null)
                    {
                        log.LogInformation("\nEmail: " + obj.Email);
                        string sendTo = obj.Email;
                        string subject = "Add file to the BLOB storage";
                        string body = $"You are add file \nName: {name} \nType: {extension} \nSize: {myBlob.Length} Bytes \nUri: {uri?.ToString()}";
                        await _emailSender.SendEmailAsync(sendTo, subject, body);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                string sendTo = "{secret}";
                string subject = "Exception";
                string body = $"You are add file \nName: {name} \nType: {extension} \nSize: {myBlob.Length} Bytes \nUri: {uri?.ToString()} \n Email: null";

                await new EmailSender().SendEmailAsync(sendTo, subject, body);
                log.LogInformation($"Exception: {ex.Message}");
            }
           
           
            
            
        }
    }
}
