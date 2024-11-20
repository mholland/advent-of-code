from functools import reduce
import sys

def convert_to_binary(input):
    binary = ""
    for c in input:
        hex = int(c, 16)
        binary += bin(hex)[2:].zfill(4)
    return binary

input = ""
with open("inputs/" + sys.argv[1], encoding="utf-8") as f:
    input = convert_to_binary(f.read().strip())

def parse_header(b, ptr):
    version = int(b[ptr:ptr+3], 2)
    type_id = int(b[ptr+3:ptr+6], 2)
    return (version, type_id)

def parse_packets(binary, ptr=0):
    version_summary = 0

    version, tid = parse_header(binary, ptr)
    ptr += 6
    version_summary += version
    values = []
    if tid == 4:
        value = ""
        while True:
            group = binary[ptr:ptr+5]
            value += group[1:]
            ptr += 5
            if group[0] == "0":
                return version_summary, ptr, int(value, 2)
    else:
        ltid = binary[ptr]
        ptr += 1
        if ltid == "0":
            subpacket_len = int(binary[ptr:ptr+15], 2)
            ptr += 15
            init_ptr = ptr
            while ptr < init_ptr + subpacket_len:
                vsum, ptr, vs = parse_packets(binary, ptr)
                values.append(vs)
                version_summary += vsum
        elif ltid == "1":
            subpacket_count = int(binary[ptr:ptr+11], 2)
            ptr += 11
            count = 0
            while count < subpacket_count:
                vsum, ptr, vs = parse_packets(binary, ptr)
                version_summary += vsum
                values.append(vs)
                count += 1

    v = 0
    if tid == 0:
        v = sum(values)
    elif tid == 1:
        v = reduce(lambda x, y: x*y, values, 1)
    elif tid == 2:
        v = min(values)
    elif tid == 3:
        v = max(values)
    elif tid == 5:
        v = 1 if values[0]>values[1] else 0
    elif tid == 6:
        v = 1 if values[0]<values[1] else 0
    elif tid == 7:
        v = 1 if values[0]==values[1] else 0

    return version_summary, ptr, v

examples_one = [
    ("8A004A801A8002F478", 16),
    ("620080001611562C8802118E34", 12),
    ("C0015000016115A2E0802F182340", 23),
    ("A0016C880162017C3686B18A3D4780", 31)
]

for e in examples_one:
    binary = convert_to_binary(e[0])
    assert parse_packets(binary)[0] == e[1]

examples_two = [
    ("C200B40A82", 3),
    ("04005AC33890", 54),
    ("880086C3E88112", 7),
    ("CE00C43D881120", 9),
    ("D8005AC2A8F0", 1),
    ("F600BC2D8F", 0),
    ("9C005AC2F8F0", 0),
    ("9C0141080250320F1802104A08", 1)
]

for e in examples_two:
    binary = convert_to_binary(e[0])
    assert parse_packets(binary)[2] == e[1]

ans = parse_packets(input)
print(ans[0])
print(ans[2])