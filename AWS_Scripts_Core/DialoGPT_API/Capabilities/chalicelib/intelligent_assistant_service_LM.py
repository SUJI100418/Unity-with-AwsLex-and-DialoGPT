import boto3
import torch
from transformers import AutoModelForCausalLM, AutoTokenizer

class IntelligentAssistantService_LM:
    def __init__(self, model_name):
        self.tokenizer = AutoTokenizer.from_pretrained(model_name)
        self.model = AutoModelForCausalLM.from_pretrained(model_name)

    def send_user_text(self, input_text, before_chat_history_ids):
        input_ids = self.tokenizer.encode(input_text + self.tokenizer.eos_token, return_tensors='pt')

        if before_chat_history_ids != "":
            bot_input_ids = torch.cat([before_chat_history_ids, input_ids], dim=-1)
        else:
            bot_input_ids = input_ids

        chat_history_ids = self.model.generate(bot_input_ids, max_length=1000, pad_token_id=self.tokenizer.eos_token_id)

        response = self.tokenizer.decode(chat_history_ids[:, bot_input_ids.shape[-1]:][0], skip_special_tokens=True)

        return response, chat_history_ids