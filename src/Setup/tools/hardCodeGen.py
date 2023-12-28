#Generator created by Joshua Hunter, Intel Labs Intern

# IMPORTANT!!!! This is a dev tool for the generator. This is not used to generate the installer. 
# - This is used to generate specific sections of HARD CODE for the generator to use, but in a way that can easily be programmed to be dynamic later.

# This script is important because it auto escapes special characters for us, preserves indentation, keeps white space, and if there is a comment line it preserves words like "don't" which has an apostrophe

def escape_special_chars(line):
    escape_dict = {
        '\\': '\\\\',
        '\"': '\\\"',
        '\'': '\\\'',
        '{': '{{',
        '}': '}}'
    }
    return "".join(escape_dict.get(c, c) for c in line)

def nsis_generator_generator(input_file):
    with open('Output.py', 'w') as f:
        f.write('with open(\'nsis_script.nsi\', \'w\') as f:\n')
        with open(input_file, 'r') as input_f:
            for line in input_f:
                if line.strip().startswith(';'):  # Treats comment lines differently
                    line = line.replace('\'', '\\\'')
                else:
                    line = escape_special_chars(line)
                f.write(f'    f.write(f\'{line.rstrip()}\\n\')\n')

if __name__ == "__main__":
    nsis_generator_generator('.\hardCode.txt')