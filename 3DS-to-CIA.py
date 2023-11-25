import subprocess
import sys
from tkinter import Tk, filedialog

def install(package):
    subprocess.run([sys.executable, "-m", "pip", "install", package])

try:
    import tkinter
except ImportError:
    install("tkinter")
finally:
    from tkinter import *
    master = Tk()
    master.withdraw()

    def start():
        print("Starting...")
        subprocess.run([
            sys.executable,
            master.path23dsconv,
            '--overwrite',
            '--boot9=' + master.boot9,
            '--output=' + master.ciadir,
            master.romdir + "/*.3ds"
        ])
        input("Now check the output! Press Enter to end.")

    master.romdir = filedialog.askdirectory(initialdir="./", title="Select directory containing ROMs...")
    master.ciadir = filedialog.askdirectory(initialdir="./", title="Select output directory...")
    master.path23dsconv = filedialog.askopenfilename(initialdir="./", title="Select 3dsconv.py",
                                                    filetypes=[("Python files", "*.py")])
    master.boot9 = filedialog.askopenfilename(initialdir="./", title="Select boot9.bin", filetypes=[("BIN files", "*.bin")])
    start()
