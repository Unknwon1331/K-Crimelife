-- Vor Ausführung zwingend ein vollständiges Datenbank-Backup erstellen.
-- Dieser Patch ist für eine bestehende K-Crimelife-Datenbank gedacht.

ALTER TABLE `accounts`
  ADD COLUMN IF NOT EXISTS `Tattoo` varchar(500) DEFAULT '[]',
  ADD COLUMN IF NOT EXISTS `Absturz` int(11) NOT NULL DEFAULT 0,
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,
  MODIFY `Password` varchar(255) NOT NULL,
  MODIFY `Banzeit` timestamp NULL DEFAULT NULL,
  MODIFY `Beschreibung` varchar(255) NOT NULL DEFAULT '';

ALTER TABLE `phone_settings`
  ADD COLUMN IF NOT EXISTS `Ringtone` int(11) NOT NULL DEFAULT 1;

-- Vor dem Anlegen der eindeutigen Indizes werden Duplikate sichtbar gemacht.
SELECT `Username`, COUNT(*) AS `Anzahl`
FROM `accounts`
GROUP BY `Username`
HAVING COUNT(*) > 1;

SELECT `Social`, COUNT(*) AS `Anzahl`
FROM `accounts`
GROUP BY `Social`
HAVING COUNT(*) > 1;

SET @username_duplicates = (
  SELECT COUNT(*) FROM (
    SELECT `Username`
    FROM `accounts`
    GROUP BY `Username`
    HAVING COUNT(*) > 1
  ) AS duplicates
);
SET @username_index_exists = (
  SELECT COUNT(*)
  FROM information_schema.statistics
  WHERE table_schema = DATABASE()
    AND table_name = 'accounts'
    AND index_name = 'uq_accounts_username'
);
SET @username_index_sql = IF(
  @username_index_exists > 0,
  'SELECT ''Username-Index existiert bereits'' AS Hinweis',
  IF(
    @username_duplicates = 0,
    'ALTER TABLE `accounts` ADD UNIQUE INDEX `uq_accounts_username` (`Username`)',
    'SELECT ''Username-Duplikate zuerst bereinigen'' AS Hinweis'
  )
);
PREPARE username_index_statement FROM @username_index_sql;
EXECUTE username_index_statement;
DEALLOCATE PREPARE username_index_statement;

SET @social_duplicates = (
  SELECT COUNT(*) FROM (
    SELECT `Social`
    FROM `accounts`
    GROUP BY `Social`
    HAVING COUNT(*) > 1
  ) AS duplicates
);
SET @social_index_exists = (
  SELECT COUNT(*)
  FROM information_schema.statistics
  WHERE table_schema = DATABASE()
    AND table_name = 'accounts'
    AND index_name = 'uq_accounts_social'
);
SET @social_index_sql = IF(
  @social_index_exists > 0,
  'SELECT ''Social-Index existiert bereits'' AS Hinweis',
  IF(
    @social_duplicates = 0,
    'ALTER TABLE `accounts` ADD UNIQUE INDEX `uq_accounts_social` (`Social`)',
    'SELECT ''Social-Duplikate zuerst bereinigen'' AS Hinweis'
  )
);
PREPARE social_index_statement FROM @social_index_sql;
EXECUTE social_index_statement;
DEALLOCATE PREPARE social_index_statement;
