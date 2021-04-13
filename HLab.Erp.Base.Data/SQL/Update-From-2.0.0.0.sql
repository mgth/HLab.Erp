BEGIN;
ALTER TABLE public."Continent" ADD COLUMN IF NOT EXISTS "Code" text;
ALTER TABLE public."Continent"  ALTER COLUMN "Name" TYPE text;

UPDATE public."Continent" SET "Code" = 'AF', "Name" = '{Africa}' WHERE "Name" = 'Afrique';
UPDATE public."Continent" SET "Code" = 'NA', "Name" = '{North america}' WHERE "Name" = 'Amérique';
UPDATE public."Continent" SET "Code" = 'AS', "Name" = '{Asia}' WHERE "Name" = 'Asie';
UPDATE public."Continent" SET "Code" = 'EU', "Name" = '{Europa}' WHERE "Name" = 'Europe';
UPDATE public."Continent" SET "Code" = 'OC', "Name" = '{Oceania}' WHERE "Name" = 'Océanie';
               
INSERT INTO public."Continent" ("Name", "Code") VALUES ('{South america}', 'SA') ON CONFLICT DO NOTHING;
INSERT INTO public."Continent" ("Name", "Code") VALUES ('{Antartica}', 'AN') ON CONFLICT DO NOTHING;

UPDATE public."DataVersion" SET "Version" = '2.1.0.0' WHERE "Module" = 'HLab.Erp.Base.Data';

COMMIT;