import socket
import struct
import threading
import tkinter as tk
from tkinter import filedialog, messagebox

def compress_file(file_path, status_label):
    try:
        status_label.config(text="Connecting to server...")
        client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        client.connect(('127.0.0.1', 9999))

        with open(file_path, 'rb') as f:
            data = f.read()

        file_size = len(data)
        status_label.config(text="Sending and compressing...")
        
        client.sendall(struct.pack('>Q', file_size))
        client.sendall(data)

        raw_size = client.recv(8)
        comp_size = struct.unpack('>Q', raw_size)[0]

        comp_data = b""
        while len(comp_data) < comp_size:
            chunk = client.recv(4096)
            if not chunk:
                break
            comp_data += chunk

        client.close()

        save_path = filedialog.asksaveasfilename(defaultextension=".zlib")
        if save_path:
            with open(save_path, 'wb') as f:
                f.write(comp_data)
            status_label.config(text="Done!")
            messagebox.showinfo("Success", f"Original Size: {file_size} bytes\nCompressed Size: {comp_size} bytes")
        else:
            status_label.config(text="Ready")

    except Exception as e:
        status_label.config(text="Connection Error.")
        messagebox.showerror("Error", str(e))

def browse_file(status_label):
    file_path = filedialog.askopenfilename()
    if file_path:
        threading.Thread(target=compress_file, args=(file_path, status_label)).start()

def main():
    root = tk.Tk()
    root.title("File Compressor Client")
    root.geometry("350x150")

    lbl = tk.Label(root, text="Select a file to compress", pady=10)
    lbl.pack()

    status = tk.Label(root, text="Ready", fg="blue")
    status.pack(pady=5)

    btn = tk.Button(root, text="Browse & Compress", command=lambda: browse_file(status))
    btn.pack(pady=10)

    root.mainloop()

if __name__ == "__main__":
    main()