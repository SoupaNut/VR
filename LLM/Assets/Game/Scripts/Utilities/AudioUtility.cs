using UnityEngine;
using UnityEngine.Audio;
using System.Text;
using System;
using System.IO;
using System.Diagnostics;

namespace Unity.Game.Utilities
{
    public class AudioUtility : MonoBehaviour
    {
        const string WhisperPath = "/Python/Python311/Scripts/whisper.exe";


        public static string GetTextFromAudioClip(int clipPosition, AudioClip clip, int sampleWindow = 64)
        {
            int startPosition = clipPosition - sampleWindow;
            float[] waveData = new float[sampleWindow];
            clip.GetData(waveData, startPosition);

            StreamAudioData(waveData);

            //if (startPosition < 0)
            //    return "";

            return "";
        }


        public static float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip, int sampleWindow = 64)
        {
            int startPosition = clipPosition - sampleWindow;
            float[] waveData = new float[sampleWindow];
            clip.GetData(waveData, startPosition);

            if (startPosition < 0)
                return 0;

            // compute loudness
            float totalLoudness = 0;
            for(int i = 0; i < sampleWindow; i++)
            {
                totalLoudness += Mathf.Abs(waveData[i]);
            }

            return totalLoudness / sampleWindow;
        }

        static void StreamAudioData(float[] data)
        {
            // Set up process start info
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = WhisperPath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true, // Redirect standard output
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Create and start the process
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                // Write audio data to the process standard input
                using (BinaryWriter binaryWriter = new BinaryWriter(process.StandardInput.BaseStream))
                {
                    // Convert float[] samples to byte[]
                    byte[] byteArray = new byte[data.Length * sizeof(float)];
                    Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);

                    // Write the byte[] to the process standard input
                    binaryWriter.Write(byteArray);
                }

                // Read the output from the process
                StringBuilder outputBuilder = new StringBuilder();
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    outputBuilder.AppendLine(line);
                    UnityEngine.Debug.Log("Output: " + line); // Print the output to Unity console
                }

                // Wait for the process to finish
                process.WaitForExit();
            }

        }
    }
}

