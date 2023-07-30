import datetime
from tortoise import Model, fields

class users(Model):
    id = fields.IntField(pk=True)
    telegram_id = fields.IntField()
    sub_day = fields.IntField()
    money = fields.FloatField()
    config = fields.JSONField(default={
        "chat_id": "0",
        "btc": "0",
        "eth": "0",
        "xmr": "0",
        "xlm": "0",
        "xrp": "0",
        "ltc": "0",
        "bch": "0"
    })


    class Meta:
        table = "users"


class check(Model):
    id = fields.IntField(pk=True)
    user_id = fields.IntField()
    bill_id = fields.TextField()
    money = fields.IntField()

    class Meta:
        table = "check"