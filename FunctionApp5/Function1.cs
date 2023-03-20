using System;
using System.IO;
using System.Threading.Tasks;
using FunctionApp5.Data;
using FunctionApp5.Repository;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FunctionApp5
{
    public class Function1
    {
       
        [FunctionName("Function1")]
        public async Task RunAsync([BlobTrigger("files-container/{name}.{extension}", Connection = "BlobConnectionString")] Stream myBlob, Uri uri, string name, string extension, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes\n {Environment.GetEnvironmentVariable("DefaultConnection")}");

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var _emailSender = new EmailSender();


            using (var _applicationContext = new ApplicationContext(Environment.GetEnvironmentVariable("DefaultConnection")))
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
                else
                {
                    log.LogInformation("\nEmail: null");

                    string sendTo = "olegkrava7@gmail.com";
                    string subject = "Add file to the BLOB storage";
                    string body = $"You are add file \nName: {name} \nType: {extension} \nSize: {myBlob.Length} Bytes \nUri: {uri?.ToString()} \n Email: null";

                    await _emailSender.SendEmailAsync(sendTo, subject, body);

                }
            }
        }
    }
}
