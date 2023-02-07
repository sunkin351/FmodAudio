﻿namespace FmodAudio;

public static class Preset
{
    public static ReverbProperties Off { get => new ReverbProperties(1000, 7, 11, 5000, 100, 100, 100, 250, 0, 20, 96, -80.0f); }
    public static ReverbProperties Generic { get => new ReverbProperties(1500, 7, 11, 5000, 83, 100, 100, 250, 0, 14500, 96, -8.0f); }
    public static ReverbProperties PaddedCell { get => new ReverbProperties(170, 1, 2, 5000, 10, 100, 100, 250, 0, 160, 84, -7.8f); }
    public static ReverbProperties Room { get => new ReverbProperties(400, 2, 3, 5000, 83, 100, 100, 250, 0, 6050, 88, -9.4f); }
    public static ReverbProperties Bathroom { get => new ReverbProperties(1500, 7, 11, 5000, 54, 100, 60, 250, 0, 2900, 83, 0.5f); }
    public static ReverbProperties LivingRoom { get => new ReverbProperties(500, 3, 4, 5000, 10, 100, 100, 250, 0, 160, 58, -19.0f); }
    public static ReverbProperties StoneRoom { get => new ReverbProperties(2300, 12, 17, 5000, 64, 100, 100, 250, 0, 7800, 71, -8.5f); }
    public static ReverbProperties Auditorium { get => new ReverbProperties(4300, 20, 30, 5000, 59, 100, 100, 250, 0, 5850, 64, -11.7f); }
    public static ReverbProperties ConcertHall { get => new ReverbProperties(3900, 20, 29, 5000, 70, 100, 100, 250, 0, 5650, 80, -9.8f); }
    public static ReverbProperties Cave { get => new ReverbProperties(2900, 15, 22, 5000, 100, 100, 100, 250, 0, 20000, 59, -11.3f); }
    public static ReverbProperties Arena { get => new ReverbProperties(7200, 20, 30, 5000, 33, 100, 100, 250, 0, 4500, 80, -9.6f); }
    public static ReverbProperties Hanger { get => new ReverbProperties(10000, 20, 30, 5000, 23, 100, 100, 250, 0, 3400, 72, -7.4f); }
    public static ReverbProperties CarpettedHallway { get => new ReverbProperties(300, 2, 30, 5000, 10, 100, 100, 250, 0, 500, 56, -24.0f); }
    public static ReverbProperties Hallway { get => new ReverbProperties(1500, 7, 11, 5000, 59, 100, 100, 250, 0, 7800, 87, -5.5f); }
    public static ReverbProperties StoneCorridor { get => new ReverbProperties(270, 13, 20, 5000, 79, 100, 100, 250, 0, 9000, 86, -6.0f); }
    public static ReverbProperties Alley { get => new ReverbProperties(1500, 7, 11, 5000, 86, 100, 100, 250, 0, 8300, 80, -9.8f); }
    public static ReverbProperties Forest { get => new ReverbProperties(1500, 162, 88, 5000, 54, 79, 100, 250, 0, 760, 94, -12.3f); }
    public static ReverbProperties City { get => new ReverbProperties(1500, 7, 11, 5000, 67, 50, 100, 250, 0, 4050, 66, -26.0f); }
    public static ReverbProperties Mountains { get => new ReverbProperties(1500, 300, 100, 5000, 21, 27, 100, 250, 0, 1220, 82, -24.0f); }
    public static ReverbProperties Quarry { get => new ReverbProperties(1500, 61, 25, 5000, 83, 100, 100, 250, 0, 3400, 100, -5.0f); }
    public static ReverbProperties Plain { get => new ReverbProperties(1500, 179, 100, 5000, 50, 21, 100, 250, 0, 1670, 65, -28.0f); }
    public static ReverbProperties ParkingLot { get => new ReverbProperties(1700, 8, 12, 5000, 100, 100, 100, 250, 0, 20000, 56, -19.5f); }
    public static ReverbProperties SewerPipe { get => new ReverbProperties(2800, 14, 21, 5000, 14, 80, 60, 250, 0, 3400, 66, 1.2f); }
    public static ReverbProperties UnderWater { get => new ReverbProperties(1500, 7, 11, 5000, 10, 100, 100, 250, 0, 500, 92, 7.0f); }
}
