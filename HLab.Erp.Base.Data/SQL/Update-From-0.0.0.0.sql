UPDATE "Icon" SET "Foreground" = -16777216 WHERE not "Path" LIKE '%/Flag/%';
UPDATE "Icon" SET "Foreground" = null WHERE "Path" LIKE '%/Flag/%';
