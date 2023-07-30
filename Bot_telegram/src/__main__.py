from src.config import dp
from aiogram.utils import executor
from src.db_ustils import connect_database

async def bot_startup(_):
    print("[+]Bot started!")
    await connect_database()
async def bot_shutdown(_):
    print("[-]Bot shutdown!")

if __name__ == "__main__":
    executor.start_polling(dispatcher=dp, on_startup=bot_startup, on_shutdown=bot_shutdown)