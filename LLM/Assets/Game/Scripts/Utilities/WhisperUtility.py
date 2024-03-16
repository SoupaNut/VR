import sys
import whisper

MODEL_SIZE = "medium"

def audio_file_to_text(filepath):
    model = whisper.load_model(MODEL_SIZE)
    result = model.transcribe(filepath)
    return result["text"]


if __name__ == "__main__"
    # Check if argument is provided
    if len(sys.argv) != 2:
        print("Usage: WhisperUtility.py <argument>")
        sys.exit(1)

    # Get argument
    filepath = sys.argv[1]

    print(audio_file_to_text(filepath))