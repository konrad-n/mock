#!/usr/bin/env python3

import os
import re
import sys

def fix_file(filepath):
    """Fix common syntax errors in a C# file"""
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
        
        original_content = content
        
        # Fix double closing parentheses ))
        content = re.sub(r'\)\)', ')', content)
        
        # Fix lines ending with (, by adding );
        content = re.sub(r'\($\s*(?=\n)', '();', content, flags=re.MULTILINE)
        
        # Fix semicolons after closing braces
        content = re.sub(r'\};\s*$', '}', content, flags=re.MULTILINE)
        
        # Fix missing semicolons on variable declarations that end with )
        content = re.sub(r'(= new \w+<[^>]+>\(\))\s*$', r'\1;', content, flags=re.MULTILINE)
        
        # Fix double commas at the end of method calls
        content = re.sub(r',\s*,', ',', content)
        
        # Fix lines that have method calls ending with just (,
        content = re.sub(r'(\.\w+)\($\s*(?=\n)', r'\1();', content, flags=re.MULTILINE)
        
        # Fix lines ending with incomplete lambda expressions
        content = re.sub(r'=>\)\s*$', '=>', content, flags=re.MULTILINE)
        
        # Fix lines ending with ), that should just be )
        content = re.sub(r'\),\s*$', ')', content, flags=re.MULTILINE)
        
        # Fix extra closing braces at the end of files
        content = re.sub(r'}\s*}\s*$', '}', content)
        
        # Fix "No newline at end of file" patterns
        content = re.sub(r'}\s*No newline at end of file\s*}\s*No newline at end of file', '}', content)
        content = re.sub(r'No newline at end of file', '', content)
        
        # Write back only if changed
        if content != original_content:
            with open(filepath, 'w', encoding='utf-8') as f:
                f.write(content)
            return True
        return False
    except Exception as e:
        print(f"Error processing {filepath}: {e}")
        return False

def main():
    app_dir = "/home/ubuntu/projects/mock/SledzSpecke.WebApi/src/SledzSpecke.Application"
    
    fixed_count = 0
    total_count = 0
    
    for root, dirs, files in os.walk(app_dir):
        for file in files:
            if file.endswith('.cs'):
                filepath = os.path.join(root, file)
                total_count += 1
                if fix_file(filepath):
                    fixed_count += 1
                    print(f"Fixed: {filepath}")
    
    print(f"\nFixed {fixed_count} out of {total_count} files")

if __name__ == "__main__":
    main()