BEGIN;
ALTER TABLE public."Icon"
    ADD COLUMN IF NOT EXISTS "Foreground" integer;

UPDATE "Icon" SET "Foreground" = -16777216 WHERE not "Path" LIKE '%/Flag/%';
UPDATE "Icon" SET "Foreground" = null WHERE "Path" LIKE '%/Flag/%';

UPDATE public."DataVersion" SET "Version" = '2.0.0.0' WHERE "Module" = 'HLab.Erp.Base.Data';
COMMIT;