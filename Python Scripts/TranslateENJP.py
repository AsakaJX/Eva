import os
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3' 
import tensorflow as tf

from transformers import AutoTokenizer, AutoModelForSeq2SeqLM, pipeline
from langdetect import detect

# available models: 'facebook/nllb-200-distilled-600M', 'facebook/nllb-200-1.3B', 'facebook/nllb-200-distilled-1.3B', 'facebook/nllb-200-3.3B'
model_name = 'facebook/nllb-200-distilled-600M'

model = AutoModelForSeq2SeqLM.from_pretrained(model_name)
tokenizer = AutoTokenizer.from_pretrained(model_name)

source = 'eng_Latn' # English
target = 'kor_Hang' # Korean
translator = pipeline('translation', model=model, tokenizer=tokenizer, src_lang=source, tgt_lang=target)

text = 'Hi, nice to meet you'
print(detect(text))

output = translator(text, max_length=400)

translated_text = output[0]['translation_text']

print(translated_text) # '안녕하세요, 반가워요'