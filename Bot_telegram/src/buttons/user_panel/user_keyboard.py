from aiogram.types import ReplyKeyboardRemove, ReplyKeyboardMarkup, KeyboardButton, InlineKeyboardMarkup, InlineKeyboardButton

b1 = KeyboardButton("Профиль")
b2 = KeyboardButton("Купить")
b3 = KeyboardButton("Поддержка")
b4 = KeyboardButton("FAQ")
b5 = KeyboardButton("Настройка")
b6 = KeyboardButton("Стиллер")
kb_client = ReplyKeyboardMarkup(resize_keyboard=True)
kb_client.row(b1,b5, b6).add(b2, b3, b4)
#Приветственные кнопки (/start)


def buy_menu(isUrl=True, url="", bill=""):
    qiwiMeni = InlineKeyboardMarkup(row_width=1)
    if isUrl:
        btnUrlQIWI = InlineKeyboardButton(text="Ссылка на оплату", url=url)
        qiwiMeni.insert(btnUrlQIWI)

    btnCheckQIWI = InlineKeyboardButton(text="Проверить оплату", callback_data="check_"+bill)
    qiwiMeni.insert(btnCheckQIWI)
    return qiwiMeni

