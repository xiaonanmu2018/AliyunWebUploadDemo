
using AliyunWebUploadDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Tea;

namespace AliyunWebUploadDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /**
      * 使用AK&SK初始化账号Client
      * @param accessKeyId
      * @param accessKeySecret
      * @return Client
      * @throws Exception
      */
        public static AlibabaCloud.SDK.Sts20150401.Client CreateClient(string accessKeyId, string accessKeySecret)
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                // 您的 AccessKey ID
                AccessKeyId = accessKeyId,
                // 您的 AccessKey Secret
                AccessKeySecret = accessKeySecret,                 
            };
            // 访问的域名
            config.Endpoint = "sts.cn-beijing.aliyuncs.com";
            return new AlibabaCloud.SDK.Sts20150401.Client(config);
        }

        public IActionResult GetSTSToken()
        {
            AlibabaCloud.SDK.Sts20150401.Client client = CreateClient("accessKeyId", "accessKeySecret");
            AlibabaCloud.SDK.Sts20150401.Models.AssumeRoleRequest assumeRoleRequest = new AlibabaCloud.SDK.Sts20150401.Models.AssumeRoleRequest() {
                DurationSeconds = 1200,
                RoleArn = "RoleArn",
                RoleSessionName = "TestUploadSession",
            };
            AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
     
            try
            {
                // 复制代码运行请自行打印 API 的返回值
               var rep= client.AssumeRoleWithOptions(assumeRoleRequest, runtime);

                return Ok(rep.Body.Credentials);
            }
            catch (TeaException error)
            {
                // 如有需要，请打印 error
               var msg=  AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
                _logger.LogInformation(msg);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                var msg = AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);

                _logger.LogInformation(msg);
            }


            return Ok();
        }
    }
}
