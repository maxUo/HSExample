﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using PropertyChanged;
using Xamarin.Forms;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using HseSampleProject13.CogntiveServices;

namespace HseSampleProject13.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class OwlsViewModel
    {
        public OwlsViewModel()
        {
            GetImageAndRunCommand = new Command(GetImageAndRun);
            SetImageAndRunCommand = new Command(SetImageAndRun);
        }

        #region Fields
        public ImageSource ImageSource { get; set; }
        public string ResultText { get; set; }
        public bool ResultIsVisible { get; set; }
        public bool IndicatorIsRunning { get; set; }
        public int ResultFontSize { get; set; }
        #endregion

        public ICommand GetImageAndRunCommand { get; set; }

        public ICommand SetImageAndRunCommand { get; set; }

        private async void SetImageAndRun()
        {
            try
            {
                ResultIsVisible = false;
                IndicatorIsRunning = true;
                ResultFontSize = 16;
                var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (cameraStatus != PermissionStatus.Granted)
                {
                    var semaphore = new SemaphoreSlim(1, 1);
                    semaphore.Wait();
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                    cameraStatus = results[Permission.Camera];
                    semaphore.Release();
                }
                if (cameraStatus == PermissionStatus.Granted)
                {
                    await CrossMedia.Current.Initialize();
                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        ResultText = ":( No camera available.";
                    }
                    else
                    {
                        var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        {
                            Directory = "SampleDirectory",
                            Name = "test.jpg"
                        });
                        if (file == null)
                            return;

                        ImageSource = ImageSource.FromFile(file.Path);
                        var temp = await MakePredictionRequest(file.Path);

                        //Делает махинации с Vision по сделанной фото
                        Vision t = new Vision();
                        ResultText = await t.MakeSomeSummary(file.GetStream());

                        //Здесь я подумал, что делать еще одну страницу мне лень
                        //Ибо функционал будет отличаться на 2 строчки,
                        //Поэтому чтобы распознавать совушек раскоментируйте нижнюю
                        //А чтобы оставить Vision не трогайте верхние 2

                        //Делает махинации с совушками по сделанной фото
                        //SetPropsAfterPredictin(temp);

                        file.Dispose();
                    }
                }
                ResultIsVisible = true;
                IndicatorIsRunning = false;
            }
            catch (Exception ex)
            {
                ResultIsVisible = true;
                IndicatorIsRunning = false;
                ResultText = ex.ToString();
                //TODO Повесить обработчики
                //Debug.WriteLine(ex);
            }
        }

        private async void GetImageAndRun()
        {
            try
            {
                ResultIsVisible = false;
                IndicatorIsRunning = true;
                ResultFontSize = 16;
                var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (storageStatus != PermissionStatus.Granted)
                {
                    var semaphore = new SemaphoreSlim(1, 1);
                    semaphore.Wait();
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    storageStatus = results[Permission.Storage];
                    semaphore.Release();
                }
                if (storageStatus == PermissionStatus.Granted)
                {
                    await CrossMedia.Current.Initialize();

                    var file = await CrossMedia.Current.PickPhotoAsync();
                    if (file == null)
                        return;

                    ImageSource = ImageSource.FromFile(file.Path);
                    var temp = await MakePredictionRequest(file.Path);

                    file.Dispose();
                    SetPropsAfterPredictin(temp);
                }
                ResultIsVisible = true;
                IndicatorIsRunning = false;
            }
            catch (Exception ex)
            {
                ResultIsVisible = true;
                IndicatorIsRunning = false;
                ResultText = ex.ToString();
                //TODO Повесить обработчики
                //Debug.WriteLine(ex);
            }
        }

        private void SetPropsAfterPredictin(Model.Model temp)
        {
            if (temp != null)
            {
                var senior = temp.Predictions.FindIndex(x => x.Tag == "senior");
                var junior = temp.Predictions.FindIndex(x => x.Tag == "junior");
                var kote = temp.Predictions.FindIndex(x => x.Tag == "kote");
                if ((temp.Predictions[kote].Probability > temp.Predictions[senior].Probability) &&
                    (temp.Predictions[kote].Probability > temp.Predictions[junior].Probability))
                {
                    ResultIsVisible = true;
                    IndicatorIsRunning = false;
                    //ImageSource = ImageSource.FromFile("triger.jpg");
                    ResultText = "KOTE !!!\n TRIGGERED";
                    ResultFontSize = 72;

                }
                else if (temp.Predictions[senior].Probability > temp.Predictions[junior].Probability)
                {
                    ResultText = "Результат: Senior Xamarin Dev " + Math.Round(temp.Predictions[senior].Probability * 100) + "%";
                }
                else if (temp.Predictions[senior].Probability < temp.Predictions[junior].Probability)
                {
                    ResultText = "Результат: Junior Xamarin Dev " +
                                  Math.Round(temp.Predictions[junior].Probability * 100) + "%";
                }
                else
                {
                    ResultText = "Non Response";
                }
            }
        }

        private static async Task<byte[]> GetImageAsByteArray(string imageFilePath)
        {
            var file = await PCLStorage.FileSystem.Current.LocalStorage.GetFileAsync(imageFilePath);
            byte[] result;
            using (Stream fileStream = await file.OpenAsync(PCLStorage.FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                result = binaryReader.ReadBytes((int) fileStream.Length);
            }
            return result;
        }

        static async Task<Model.Model> MakePredictionRequest(string imageFilePath)
        {
            var client = new HttpClient();
            string result = "";
            // Request headers - replace this example key with your valid subscription key.
            client.DefaultRequestHeaders.Add("Prediction-Key", "63d06b337b234119b157e0a73e4539b7");
            // Prediction URL - replace this example URL with your valid prediction URL.
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/78597406-de75-4e16-b8ef-6bf693d3eaf8/image";

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored image.
            byte[] byteData = await GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                result = await response.Content.ReadAsStringAsync();
            }
            Model.Model myObj = JsonConvert.DeserializeObject<Model.Model>(result);
            return myObj;
        }
    }
}
