import boto3

class IntelligentAssistantService:
    def __init__(self, assistant_name):
        self.client = boto3.client('lex-runtime')
        self.assistant_name = assistant_name

    def send_user_text(self, user_id, input_text):
        response = self.client.post_text(
            botName = self.assistant_name,
            botAlias = 'demo',  # 봇 빌드 별칭
            userId = user_id,
            inputText = input_text
        )

        intentName = "NULL"

        if 'intentName' in response:
            intentName = response['intentName']
        else:
            intentName = "NULL"

        return response['message'] + ":" + intentName

#response['message'] + ":" + response['intentName'] + ":" + input_text