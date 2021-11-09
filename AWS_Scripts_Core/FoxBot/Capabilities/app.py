# -*- coding: utf-8 -*-

from chalice import Chalice
from chalicelib import intelligent_assistant_service
from chalicelib import translation_service

import json

#####
# 챌리스 애플리케이션 설정
#####
app = Chalice(app_name='Capabilities')
app.debug = True

#####
# 서비스 초기화
#####
assistant_name = 'FoxBot'
assistant_service = intelligent_assistant_service.IntelligentAssistantService(assistant_name)
translation_service = translation_service.TranslationService()

#####
# RESTful 엔드포인트
#####
@app.route('/'엔드 포인트 이슈로 생략'/user-id/{user_id}/send-text', methods = ['POST'], cors = True)
def send_user_text(user_id):
    request_data = json.loads(app.current_request.raw_body)
    raw_text = request_data['text']  # 인풋 json 필드 이름 변경하지 말것
    translated_text = translation_service.translate_text(raw_text)

    message = assistant_service.send_user_text(user_id, translated_text['translatedText'])

    return message + ":" + translated_text['translatedText']