using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace HseSampleProject13.CogntiveServices
{
    public class Vision
    {
        //Sample keys
        //Viz: 8ed69c4b8d82433aafde0f947360ff9e
        //Trans: 6e32a3dbe36546b8ae02b31eeb2cd904

        private VisionServiceClient _visionServiceClient;

        private AnalysisResult _analysisResult;

        private VisualFeature[] _visualFeatures;

        private readonly string _visionAPISubscriptionString = "8ed69c4b8d82433aafde0f947360ff9e";

        private readonly string _transtaleAPISubscriptionString = "6e32a3dbe36546b8ae02b31eeb2cd904";

        private bool _isVision;

        public AnalysisResult AnalysisResult { private set; get; }

        public bool IsVision { private set; get; }

        public Vision()
        {
            _visionServiceClient = new VisionServiceClient(_visionAPISubscriptionString, 
                                                           "https://westus.api.cognitive.microsoft.com/vision/v1.0");
            _isVision = false;
            _visualFeatures = new VisualFeature[] {
                                                    VisualFeature.Adult,
                                                    VisualFeature.Categories,
                                                    VisualFeature.Color,
                                                    VisualFeature.Description,
                                                    VisualFeature.Faces,
                                                    VisualFeature.ImageType,
                                                    VisualFeature.Tags
                                                   };
        }

        private async Task StartRecognize(Stream photo)
        {
            try
            {
                _analysisResult = await _visionServiceClient.DescribeAsync(photo);
                _isVision = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public async Task<string> MakeSomeSummary(Stream photo)
        {
            string result = "nothing";
            await StartRecognize(photo);
            if (_isVision)
            {
                if (_analysisResult.Description.Captions.Length > 0)
                {
                    result = _analysisResult.Description.Captions[0].Text;
                }
            }

            return await TranslateText(result, "ru", await GetAuthenticationToken(_transtaleAPISubscriptionString));
        }

        private async Task<string> TranslateText(string inputText, string language, string accessToken)
        {
            string result = "";
            string url = "http://api.microsofttranslator.com/v2/Http.svc/Translate";
            string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&to={language}&contentType=text/plain";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(url + query);
                result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    result = "Hata: " + result;
                }
                else
                {
                    result = XElement.Parse(result).Value;
                }

            }
            return result;
        }

        private async Task<string> GetAuthenticationToken(string key)
        {
            string endpoint = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                var response = await client.PostAsync(endpoint, null);
                var token = await response.Content.ReadAsStringAsync();
                return token;
            }
        }

    }
}
