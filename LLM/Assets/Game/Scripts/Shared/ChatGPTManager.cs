using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using OpenAI;

namespace Unity.Game.Shared
{
    public class ChatGPTManager : MonoBehaviour
    {
        [TextArea(5, 20)]
        public string Personality;

        [TextArea(5, 20)]
        public string Scene;

        public int MaxResponseWordLimit = 15;

        public List<NPCAction> actions;

        [System.Serializable]
        public struct NPCAction
        {
            public string actionKeyword;
            [TextArea(2, 5)]
            public string actionDescription;

            public UnityEvent actionEvent;
        }

        public OnResponseEvent OnResponse;

        [System.Serializable]
        public class OnResponseEvent : UnityEvent<string> { }

        OpenAIApi openai = new OpenAIApi();
        List<ChatMessage> messages = new List<ChatMessage>();

        string ChatGPTModel = "gpt-3.5-turbo";
        string WhisperModel = "whisper-1";
        string WhisperLanguage = "en";

        public string GetInstructions()
        {
            string instructions = "You are a video game character and will answer to the message the player ask you. \n" +
            "You must reply to the player message only using the information from your Personality and the Scene that are provided afterwards. \n" +
            "Do not invent or create response that are not mentioned in these information. \n" +
            "Do not break character or mention you are an AI or a video game character. \n" +
            "You must answer in less than " + MaxResponseWordLimit + "words. \n" +

            "Here is the information about your Personality : \n" +

            Personality + "\n" +

            "Here is the information about the Scene around you : \n" +

            Scene + "\n" +

            BuildActionInstructions() + 

            "Here is the message of the player : \n";

            return instructions;
        }

        public string BuildActionInstructions()
        {
            string instructions = "";
            
            foreach (var item in actions)
            {
                instructions += "If I imply that I want you to do the following: " + item.actionDescription
                    + ". You must add to your answer the following keyword, no exceptions: " + item.actionKeyword + ".\n";
            }

            return instructions;
        }

        public async void AskChatGPT(string newText)
        {
            ChatMessage newMessage = new ChatMessage();
            newMessage.Content = GetInstructions() + newText;
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

                Debug.Log("ChatGPT: " + chatResponse.Content);

                foreach (var item in actions)
                {
                    if(chatResponse.Content.Contains(item.actionKeyword))
                    {
                        string textNoKeyword = chatResponse.Content.Replace(item.actionKeyword, "");
                        chatResponse.Content = textNoKeyword;

                        item.actionEvent.Invoke();
                    }
                }

                messages.Add(chatResponse);

                OnResponse.Invoke(chatResponse.Content);
                
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


