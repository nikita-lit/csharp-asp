import re
from pathlib import Path

root = Path(r"c:\FILES\DEV\Sources\csharp-asp\School")
views = list(root.glob('Views/**/*.cshtml'))
resx_en = root / 'Resources' / 'LanguageResources.en.resx'
resx_et = root / 'Resources' / 'LanguageResources.et.resx'
resx_ru = root / 'Resources' / 'LanguageResources.ru.resx'

key_pattern = re.compile(r'Localizer\["([^"\]]+)"\]')
keys_used = set()
for p in views:
    text = p.read_text(encoding='utf-8')
    for m in key_pattern.findall(text):
        keys_used.add(m)


def resx_keys(path):
    s = path.read_text(encoding='utf-8')
    return set(re.findall(r'<data name="([^"]+)"', s))

keys_en = resx_keys(resx_en)
keys_et = resx_keys(resx_et)
keys_ru = resx_keys(resx_ru)

missing_en = sorted(list(keys_used - keys_en))
missing_et = sorted(list(keys_used - keys_et))
missing_ru = sorted(list(keys_used - keys_ru))

out = {'used_keys_count': len(keys_used), 'missing_en': missing_en, 'missing_et': missing_et, 'missing_ru': missing_ru}
print(out)
with open(root / 'tools' / 'missing_keys.json','w',encoding='utf-8') as f:
    import json
    json.dump(out,f,ensure_ascii=False,indent=2)
print('Wrote tools/missing_keys.json')
