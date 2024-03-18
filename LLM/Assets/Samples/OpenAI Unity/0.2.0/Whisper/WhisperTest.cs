//using OpenAI;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System;
using System.IO;
using System.Text;
using System.Collections;

namespace Samples.Whisper
{
    public class WhisperTest : MonoBehaviour
    {
        [SerializeField] private Button recordButton;
        [SerializeField] private Image progressBar;
        [SerializeField] private Text message;
        [SerializeField] private Dropdown dropdown;
        
        private readonly string fileName = "output.wav";
        private readonly int duration = 5;
        
        private AudioClip clip;
        private bool isRecording;
        private float time;
        //private OpenAIApi openai = new OpenAIApi();

        const string WhisperPath = "/Python/Python311/Scripts/whisper.exe";
        const string PyFilePath = "\\GitHub\\VR\\LLM\\Assets\\Game\\Scripts\\Utilities\\WhisperUtility.py";

        private void Start()
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
            #else
            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
            dropdown.onValueChanged.AddListener(ChangeMicrophone);
            
            var index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);
            #endif
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }
        
        private void StartRecording()
        {
            isRecording = true;
            recordButton.enabled = false;

            var index = PlayerPrefs.GetInt("user-mic-device-index");
            
            #if !UNITY_WEBGL
            clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
            #endif
        }

        private async void EndRecording()
        {
            UnityEngine.Debug.Log("Starting time: " + DateTime.Now);
            message.text = "Transcripting...";
            
            #if !UNITY_WEBGL
            Microphone.End(null);
#endif

            byte[] data = SaveWav.Save(fileName, clip);

            // create file from data bytes
            var audioFilePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(audioFilePath, data);

            StartCoroutine(GetTextFromSpeech(audioFilePath));

            //ProcessStartInfo startInfo = new ProcessStartInfo();

            //startInfo.FileName = "python"; // assume python is in PATH environment variables
            //startInfo.Arguments = $"\"{PyFilePath}\" \"{audioFilePath}\"";
            //startInfo.RedirectStandardInput = true;
            //startInfo.RedirectStandardOutput = true;
            //startInfo.UseShellExecute = false;
            //startInfo.CreateNoWindow = true;

            //using (Process process = new Process())
            //{
            //    process.StartInfo = startInfo;
            //    process.Start();

            //    using (StreamReader reader = process.StandardOutput)
            //    {
            //        string result = reader.ReadToEnd();
            //        UnityEngine.Debug.Log(result);
            //    }
            //}

            //UnityEngine.Debug.Log("Time it took: " + (Time.time - startTime));

            //using(Process process = new Process())
            //{
            //    process.StartInfo = startInfo;
            //    process.Start();

            //    StringBuilder outputBuilder = new StringBuilder();
            //    while (!process.StandardOutput.EndOfStream)
            //    {
            //        string line = process.StandardOutput.ReadLine();
            //        outputBuilder.AppendLine(line);
            //        UnityEngine.Debug.Log("Output: " + line); // Print the output to Unity console
            //    }

            //    // Wait for the process to finish
            //    process.WaitForExit();
            //}

            //var req = new CreateAudioTranscriptionsRequest
            //{
            //    FileData = new FileData() {Data = data, Name = "audio.wav"},
            //    // File = Application.persistentDataPath + "/" + fileName,
            //    Model = "whisper-1",
            //    Language = "en"
            //};
            //var res = await openai.CreateAudioTranscription(req);

            progressBar.fillAmount = 0;
            //message.text = res.Text;
            recordButton.enabled = true;
        }

        IEnumerator GetTextFromSpeech(string audioFilePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.FileName = "python"; // assume python is in PATH environment variables
            startInfo.Arguments = $"\"{PyFilePath}\" \"{audioFilePath}\"";
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    UnityEngine.Debug.Log(result);
                }
            }

            yield return null;
        }

        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
                progressBar.fillAmount = time / duration;
                
                if (time >= duration)
                {
                    time = 0;
                    isRecording = false;
                    EndRecording();
                }
            }
        }
    }
}
