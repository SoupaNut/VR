# import sys
import requests
import json

url = "http://localhost:11434/api/generate"
headers = {"Content-Type": "application/json"}
data = {
    "model": "llama2",
    "prompt": "What is water made of?"
}

response = requests.post(url, headers=headers, data=json.dumps(data))
print(response.json())

# def get_ai_response(prompt):


# if __name__ == "__main__":
#     # Check if argument is provided
#     if len(sys.argv) != 2:
#         print("Usage: WhisperUtility.py <argument>")
#         sys.exit(1)

#     # Get argument
#     prompt = sys.argv[1]

#     get_ai_response(prompt)