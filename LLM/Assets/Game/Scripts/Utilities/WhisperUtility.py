import sys
from faster_whisper import WhisperModel
# import whisper

MODEL_SIZE = "medium.en"

def audio_file_to_text(filepath):
    model = WhisperModel(MODEL_SIZE, device="cuda", compute_type="float16")

    segments, info = model.transcribe(filepath, beam_size=5)

    # print("Detected language '%s' with probability %f" % (info.language, info.language_probability))

    i = 0
    for segment in segments:
        i += 1
        # print("[%.2fs -> %.2fs] %s" % (segment.start, segment.end, segment.text))
        print(segment.text)
    
    print(i)

    # model = whisper.load_model(MODEL_SIZE)
    # result = model.transcribe(filepath)
    # print(result["text"])
    # return result["text"]


if __name__ == "__main__":
    # Check if argument is provided
    if len(sys.argv) != 2:
        print("Usage: WhisperUtility.py <argument>")
        sys.exit(1)

    # Get argument
    filepath = sys.argv[1]

    audio_file_to_text(filepath)
    # audio_file_to_text("C:\\Users\\Tam Nguyen\\AppData\\LocalLow\\DefaultCompany\\LLM\\output.wav")