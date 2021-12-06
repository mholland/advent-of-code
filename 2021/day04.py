class Board:

    def __init__(self, board) -> None:
        self.rows = [[int(x) for x in y.split()] for y in board]
        self.numbers = dict()
        self.won = False
        for y in self.rows:
            for x in y:
                self.numbers[x] = False
    
    def mark(self, num: int) -> None:
        if num in self.numbers:
            self.numbers[num] = True

    def house(self) -> bool:
        for y in self.rows:
            if len(y) == len([x for x in y if self.numbers[x]]):
                self.won = True
                return True

        for x in range(len(self.rows[0])):
            col = True
            for y in range(len(self.rows)):
                if self.rows[y][x] not in self.numbers or not self.numbers[self.rows[y][x]]:
                    col = False
            if col:
                self.won = True
                return True

        return False

    def score(self, call: int) -> int:
        sum = 0
        for k, v in self.numbers.items():
            if not v:
                sum += k
        return sum * call

calls = None
boards = []
with open("inputs/day04.input", encoding = "utf-8") as f:
    lines = []
    for line in f:
        lines.append(line.strip())

    calls = [int(x) for x in lines[0].split(",")]
    b = []
    for i in range(2, len(lines)):
        if lines[i]:
            b.append(lines[i])
        else:
            boards.append(Board(b))
            b = []
    boards.append(Board(b))

winners = []
for call in calls:
    for b in boards:
        if b.won:
            continue
        b.mark(call)
        if b.house():
            winners.append(b.score(call))

print(winners[0])
print(winners[-1])

