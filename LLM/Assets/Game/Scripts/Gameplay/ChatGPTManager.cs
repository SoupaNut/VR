using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using System.Threading.Tasks;

namespace Unity.Game.Gameplay
{
    public class ChatGPTManager : MonoBehaviour
    {
        OpenAIApi openai = new OpenAIApi();
        List<ChatMessage> messages = new List<ChatMessage>();

        string ChatGPTModel = "gpt-3.5-turbo";
        string WhisperModel = "whisper-1";
        string WhisperLanguage = "en";

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public async void AskChatGPT(string newText)
        {
            ChatMessage newMessage = new ChatMessage();
            newMessage.Content = newText;
            newMessage.Role = "user";

            messages.Add(newMessage);

            var req = new CreateChatCompletionRequest
            {
                Messages = messages,
                Model = ChatGPTModel,
            };

            var res = await openai.CreateChatCompletion(req);

            if(res.Choices != null && res.Choices.Count > 0)
            {
                var chatResponse = res.Choices[0].Message;
                messages.Add(chatResponse);
                Debug.Log(chatResponse.Content);
            }

        }

        public async Task<string> GetAudioTranscription(byte[] data)
        {
            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() { Data = data, Name = "audio.wav" },
                // File = Application.persistentDataPath + "/" + fileName,
                Model = WhisperModel,
                Language = WhisperLanguage
            };

            var res = await openai.CreateAudioTranscription(req);

            return res.Text;
        }
    }
}


