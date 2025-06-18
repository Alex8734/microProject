using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Mx.Persistence.Model;

namespace Mx.Persistence.Util;

public static class MigrationExtensions
{
    public static void SeedData(this MigrationBuilder migrationBuilder)
    {
        // Benutzer erstellen
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "user",
            columns: new[] { "ssn", "name", "age", "weight" },
            values: new object[] { "123-45-6789", "Max Mustermann", 35, 82.5 }
        );
        
        // Tracks erstellen
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "track",
            columns: new[] { "id", "name", "length_in_km", "difficulty", "image_url" },
            values: new object[] { 1, "Hockenheimring", 4.574, (int)TrackDifficulty.Medium, "https://www.hockenheimring.de/uploads/tx_templavoila/Porsche_Turn_1_01.jpg" }
        );
        
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "track",
            columns: new[] { "id", "name", "length_in_km", "difficulty", "image_url" },
            values: new object[] { 2, "Nürburgring", 5.148, (int)TrackDifficulty.Hard, "https://nuerburgring.de/fileadmin/_processed_/b/d/csm_ring-2021-gp-2-c-gruppe-c_d758bd4726.jpg" }
        );
        
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "track",
            columns: new[] { "id", "name", "length_in_km", "difficulty", "image_url" },
            values: new object[] { 3, "Sachsenring", 3.671, (int)TrackDifficulty.Medium, "https://www.sachsenring-circuit.com/images/uploads/slider/6169b04370a5d.jpg" }
        );
        
        // Koordinaten für Hockenheimring
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "track_coordinate",
            columns: new[] { "id", "track_id", "latitude", "longitude", "sequence_number" },
            values: new object[] { 1, 1, 49.329639, 8.569189, 1 }
        );
        
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "track_coordinate",
            columns: new[] { "id", "track_id", "latitude", "longitude", "sequence_number" },
            values: new object[] { 2, 1, 49.331939, 8.566271, 2 }
        );
        
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "track_coordinate",
            columns: new[] { "id", "track_id", "latitude", "longitude", "sequence_number" },
            values: new object[] { 3, 1, 49.332767, 8.565756, 3 }
        );
        
        // Koordinaten für Nürburgring
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "track_coordinate",
            columns: new[] { "id", "track_id", "latitude", "longitude", "sequence_number" },
            values: new object[] { 4, 2, 50.335556, 6.947222, 1 }
        );
        
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "track_coordinate",
            columns: new[] { "id", "track_id", "latitude", "longitude", "sequence_number" },
            values: new object[] { 5, 2, 50.336111, 6.949444, 2 }
        );
        
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "track_coordinate",
            columns: new[] { "id", "track_id", "latitude", "longitude", "sequence_number" },
            values: new object[] { 6, 2, 50.335833, 6.951389, 3 }
        );
        
        // Koordinaten für Sachsenring
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "track_coordinate",
            columns: new[] { "id", "track_id", "latitude", "longitude", "sequence_number" },
            values: new object[] { 7, 3, 50.794167, 12.689444, 1 }
        );
        
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "track_coordinate",
            columns: new[] { "id", "track_id", "latitude", "longitude", "sequence_number" },
            values: new object[] { 8, 3, 50.796111, 12.689722, 2 }
        );
        
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "track_coordinate",
            columns: new[] { "id", "track_id", "latitude", "longitude", "sequence_number" },
            values: new object[] { 9, 3, 50.797500, 12.691111, 3 }
        );
        
        // Motorräder erstellen
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "motorcycle",
            columns: new[] { "id", "model", "number", "horsepower", "track_id", "is_rented" },
            values: new object[] { 1, "Honda CBR 1000RR-R Fireblade", "CBR-001", 217, 1, false }
        );
        
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "motorcycle",
            columns: new[] { "id", "model", "number", "horsepower", "track_id", "is_rented" },
            values: new object[] { 2, "Kawasaki Ninja ZX-10R", "ZX10-002", 203, 2, false }
        );
        
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "motorcycle",
            columns: new[] { "id", "model", "number", "horsepower", "track_id", "is_rented" },
            values: new object[] { 3, "Ducati Panigale V4", "PAN-003", 214, 3, false }
        );
        
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "motorcycle",
            columns: new[] { "id", "model", "number", "horsepower", "track_id", "is_rented" },
            values: new object[] { 4, "BMW S 1000 RR", "BMW-004", 207, 1, false }
        );
        
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "motorcycle",
            columns: new[] { "id", "model", "number", "horsepower", "track_id", "is_rented", "rented_by_ssn" },
            values: new object[] { 5, "Yamaha YZF-R1", "YZF-005", 200, 2, true, "123-45-6789" }
        );
        
        migrationBuilder.InsertData(
            schema: "Mx",
            table: "motorcycle",
            columns: new[] { "id", "model", "number", "horsepower", "track_id", "is_rented" },
            values: new object[] { 6, "Aprilia RSV4", "RSV-006", 217, 3, false }
        );
    }
}
