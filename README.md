# Unity-with-AwsLex-and-DialoGPT
(유니티 환경) AWS Lex API 와 언어 모델 DialoGPT를 사용하여 구현한 실시간 대화 시스템 Demo


![그림4](https://user-images.githubusercontent.com/46912893/140865459-3b2a0dac-4839-42ba-b0f3-d164addc5a2b.png)


### :robot: ※ AWS_INTELLIGNET_ASSISTANT_SERVICE_LM 코드


    # -*- coding: utf-8 -*-
    from chalice import Chalice
    from chalicelib import intelligent_assistant_service_LM
    from chalicelib import translation_service

    import json

    #####
    # 챌리스 애플리케이션 설정
    #####
    app = Chalice(app_name='DialoGPT_BOT')
    app.debug = True

    #####
    # 서비스 초기화
    #####
    model_name = 'microsoft/DialoGPT-medium'
    assistant_service = intelligent_assistant_service_LM.IntelligentAssistantService_LM(model_name)
    translation_service = translation_service.TranslationService()

    #####
    # RESTful 엔드포인트
    #####
    @app.route('/('end point 이슈로 생략')/send-text', methods = ['POST'], cors = True)
    def send_user_text():
        request_data = json.loads(app.current_request.raw_body)
        raw_text = request_data['text']  # 인풋 json 필드 이름 변경하지 말것
        translated_text = translation_service.translate_text(raw_text)

        before_message = ""
        message, before_message = assistant_service.send_user_text(translated_text['translatedText'],before_message)
        translated_message = translation_service.translate_text(message, 'en', 'ko')

        return translated_message['translatedText'] + ":" + translated_text['translatedText'] + ":" + message
        


---------------

### :robot: ※ IntelligentAssistantService_LM 코드

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

