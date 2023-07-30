from aiogram import Bot, Dispatcher
from aiogram.contrib.fsm_storage.memory import MemoryStorage
from aiogram.types import ParseMode
from pyqiwip2p import QiwiP2P

BOT_TOKEN = ''
p2p = QiwiP2P(auth_key="")

#ADMIN_IDS = [888012259]
storage = MemoryStorage()

api = Bot(token=BOT_TOKEN, parse_mode=ParseMode.HTML)
dp = Dispatcher(api, storage=storage)

def days_to_seconds(days):
    return days * 24 * 60 * 60