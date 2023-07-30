import sqlite3
from tortoise import Tortoise
from src.models import users, check
import time

async def connect_database()->None:
    await Tortoise.init(db_url="sqlite://resources/db.sqlite3", modules={"models": ["src.models"]})
    await Tortoise.generate_schemas()

async def add_user(user_id, sub_license=0):
    user = await users.get_or_none(telegram_id=user_id)
    if user is None:
        print("[?]Not in db!")
        new_user = await users.create(telegram_id=user_id, sub_day=sub_license, money=0)
        new_user.config["chat_id"]=str(user_id)
        ##user.config["chat_id"]=str(user_id)
        await new_user.save()
        print("[+]User added!")

async def get_money_balance(user_id):
    users_ids = await users.get_or_none(telegram_id=user_id)
    if users_ids:
        return str(users_ids.money)



async def get_teleid(user_id):
    users_ids=await users.get_or_none(telegram_id=user_id)
    if users_ids:
        return str(users_ids.telegram_id)


async def add_balance(user_id, money):
    users_ids = await users.get_or_none(telegram_id=user_id)
    if users_ids:
        users_ids.money += money
        await users_ids.save()


async def add_sub_days(user_id, days, money):
    users_ids = await users.get_or_none(telegram_id=user_id)
    if users_ids:
        users_ids.sub_day += days
        users_ids.money -= money
        await users_ids.save()


def seconds_to_days(seconds):
    weeks = seconds / (7 * 24 * 60 * 60)
    return seconds / (24*60*60) - 7*weeks

async def get_sub_days(user_id):
    users_ids = await users.get_or_none(telegram_id=user_id)
    if users_ids:
        return int(users_ids.sub_day)

async def get_sub_status(user_id):
    users_ids = await users.get_or_none(telegram_id=user_id)
    if users_ids:
        if users_ids.sub_day > int(time.time()):
            return True
        else:
            return False
### все для оплаты киви и работы с бд ЧЕЕЕК

async def add_check(user_id, bill_id, money) -> None:
    bill = await check.get_or_none(bill_id=bill_id)
    if bill is None:
        await check.create(user_id=user_id, bill_id=bill_id, money=money)

async def get_check(bill_id) -> None:
    return await check.get_or_create(bill_id=bill_id)

async def get_money_in_check(bill_id):
    check_id = await check.get_or_none(bill_id=bill_id)
    if check_id:
        return int(check_id.money)

async def del_check(bill_id) -> None:
    return await (await get_check(bill_id)).delete()

