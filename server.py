import socket
import threading
import zlib
import struct

def handle_client(conn, addr):
    try:
        raw_size = conn.recv(8)
        if not raw_size:
            return
        
        file_size = struct.unpack('>Q', raw_size)[0]
        
        received_data = b""
        while len(received_data) < file_size:
            chunk = conn.recv(4096)
            if not chunk:
                break
            received_data += chunk

        compressed_data = zlib.compress(received_data)
        compressed_size = len(compressed_data)

        conn.sendall(struct.pack('>Q', compressed_size))
        conn.sendall(compressed_data)

    except Exception:
        pass
    finally:
        conn.close()

def start_server():
    host = '127.0.0.1'
    port = 9999
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind((host, port))
    server.listen(5)
    print("Server is listening on port 9999...")

    while True:
        conn, addr = server.accept()
        thread = threading.Thread(target=handle_client, args=(conn, addr))
        thread.start()

if __name__ == "__main__":
    start_server()