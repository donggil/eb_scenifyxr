import openai
import json
openai.api_key = "your-key-here"
def parse_request(prompt):
    response = openai.ChatCompletion.create(model="gpt-4o", messages=[{"role": "user", "content": f"Parse to JSON scene graph: {prompt}"}])
    return json.loads(response.choices[0].message.content)
# Test: print(parse_request("server room with Cisco switch"))