import csv

fileName = input("Please enter a filename (I.E 'Lvl1-Wave1.csv'): ")

with open(fileName, mode='w') as wave_file:
    wave_writer = csv.writer(wave_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL, lineterminator='\n')
    totalSpawnTime = 0
    while (True):
        print("Currently Enemies will finish spawning at T = " + str(totalSpawnTime))
        spawnTime = input("Enter spawn time as int or float ( I.E if you put this as 1 the first enemy will spawn at t = 1)) Or enter q to quit: ")
        if spawnTime == 'q':
            break
        totalSpawnTime += float(spawnTime)
        enemyInput = 0
        enemyList = list()
        while enemyInput != '':
            enemyInput = input("Enter enemy number (denoted by list of enemy prefabs in unity) or enter nothing to finish enemy list: ")
            if enemyInput == '':
                break
            enemyList.append(enemyInput)
        enemyString = "|".join(map(str,enemyList))
        print(enemyString)    
        spawnGap = input("Enter time gap between enemies as float ( I.E if you put 0.5 enemies will spawn o.5 seconds after eachother): ")
        wave_writer.writerow([spawnTime, enemyString, spawnGap])