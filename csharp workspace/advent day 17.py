import itertools

containers = [33, 14, 18, 20, 45, 35, 16, 35, 1, 13, 18, 13, 50, 44, 48, 6, 24, 41, 30, 42]
count1 = 0
smolsize = len(containers)
smolcount = 0

for i in range(0, len(containers) + 1):
    for combi in itertools.combinations(containers, i):
        if sum(combi) == 150:
            count1 += 1
            if len(combi) < smolsize:
                smolsize = len(combi)
                smolcount = 1
            elif len(combi) == smolsize:
                smolcount += 1

print count1
print smolcount
