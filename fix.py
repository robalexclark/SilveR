import os
import chardet

source_dir = r"C:\Users\robal\OneDrive\Source\Silver-Local"

for root, _, files in os.walk(source_dir):
    for filename in files:
        if filename.endswith(".R"):
            file_path = os.path.join(root, filename)
            try:
                # Detect file encoding
                with open(file_path, "rb") as f:
                    raw_data = f.read()
                    detected = chardet.detect(raw_data)
                    source_encoding = detected['encoding']
                
                # Read and rewrite file with detected encoding
                with open(file_path, "r", encoding=source_encoding) as infile:
                    content = infile.read()
                with open(file_path, "w", encoding="utf-8") as outfile:
                    outfile.write(content)
            except Exception as e:
                print(f"Error processing file {file_path}: {e}")

print("Conversion completed: All HTML files converted with detected encoding.")
