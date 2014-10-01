--------------------------------------------------------
--  DDL for Table "EDM_DESIGN"
--------------------------------------------------------

  CREATE TABLE "PROCESS"."EDM_DESIGN" 
   (	
	"DESIGN_ID" NUMBER(38,0), 
	"EDM_NAME" VARCHAR2(256 BYTE), 
	"EDM_VALUE" VARCHAR2(512 BYTE), 
	"EDM_TYPE" VARCHAR2(32 BYTE)
   ) ;
--------------------------------------------------------
--  Constraints for Table "EDM_DESIGN"
--------------------------------------------------------

  ALTER TABLE "PROCESS"."EDM_DESIGN" MODIFY ("DESIGN_ID" NOT NULL ENABLE);
 
  ALTER TABLE "PROCESS"."EDM_DESIGN" MODIFY ("EDM_NAME" NOT NULL ENABLE);
 
  ALTER TABLE "PROCESS"."EDM_DESIGN" MODIFY ("EDM_VALUE" NOT NULL ENABLE);
 
  ALTER TABLE "PROCESS"."EDM_DESIGN" MODIFY ("EDM_TYPE" NOT NULL ENABLE);


--------------------------------------------------------
--  DDL for Table EDM_COMPATIBLE_UNIT
--------------------------------------------------------

  CREATE TABLE "EDM_COMPATIBLE_UNIT" 
   (	
	"COMPATIBLE_UNIT_ID" NUMBER(38,0), 
	"DESIGN_ID" NUMBER(38,0), 
	"EDM_NAME" VARCHAR2(256), 
	"EDM_VALUE" VARCHAR2(512), 
	"EDM_TYPE" VARCHAR2(32)
   )
--------------------------------------------------------
--  Constraints for Table EDM_COMPATIBLE_UNIT
--------------------------------------------------------

  ALTER TABLE "EDM_COMPATIBLE_UNIT" MODIFY ("COMPATIBLE_UNIT_ID" NOT NULL ENABLE)
 
  ALTER TABLE "EDM_COMPATIBLE_UNIT" MODIFY ("DESIGN_ID" NOT NULL ENABLE)
 
  ALTER TABLE "EDM_COMPATIBLE_UNIT" MODIFY ("EDM_NAME" NOT NULL ENABLE)
 
  ALTER TABLE "EDM_COMPATIBLE_UNIT" MODIFY ("EDM_VALUE" NOT NULL ENABLE)
 
  ALTER TABLE "EDM_COMPATIBLE_UNIT" MODIFY ("EDM_TYPE" NOT NULL ENABLE)

--------------------------------------------------------
--  DDL for Table EDM_WORK_LOCATION
--------------------------------------------------------

  CREATE TABLE "EDM_WORK_LOCATION" 
   (	
	"WORK_LOCATION_ID" NUMBER(38,0), 
	"DESIGN_ID" NUMBER(38,0), 
	"EDM_NAME" VARCHAR2(256), 
	"EDM_VALUE" VARCHAR2(512), 
	"EDM_TYPE" VARCHAR2(32)
   )
--------------------------------------------------------
--  Constraints for Table EDM_WORK_LOCATION
--------------------------------------------------------

  ALTER TABLE "EDM_WORK_LOCATION" MODIFY ("WORK_LOCATION_ID" NOT NULL ENABLE)
 
  ALTER TABLE "EDM_WORK_LOCATION" MODIFY ("DESIGN_ID" NOT NULL ENABLE)
 
  ALTER TABLE "EDM_WORK_LOCATION" MODIFY ("EDM_NAME" NOT NULL ENABLE)
 
  ALTER TABLE "EDM_WORK_LOCATION" MODIFY ("EDM_VALUE" NOT NULL ENABLE)
 
  ALTER TABLE "EDM_WORK_LOCATION" MODIFY ("EDM_TYPE" NOT NULL ENABLE)

--------------------------------------------------------
--  DDL for Table EDM_WORK_REQUEST
--------------------------------------------------------

  CREATE TABLE "EDM_WORK_REQUEST" 
   (	
	"WORK_REQUEST_ID" NUMBER(38,0), 
	"DESIGN_ID" NUMBER(38,0), 
	"EDM_NAME" VARCHAR2(256), 
	"EDM_VALUE" VARCHAR2(512), 
	"EDM_TYPE" VARCHAR2(32)
   )
--------------------------------------------------------
--  Constraints for Table EDM_WORK_REQUEST
--------------------------------------------------------

  ALTER TABLE "EDM_WORK_REQUEST" MODIFY ("WORK_REQUEST_ID" NOT NULL ENABLE)
 
  ALTER TABLE "EDM_WORK_REQUEST" MODIFY ("DESIGN_ID" NOT NULL ENABLE)
 
  ALTER TABLE "EDM_WORK_REQUEST" MODIFY ("EDM_NAME" NOT NULL ENABLE)
 
  ALTER TABLE "EDM_WORK_REQUEST" MODIFY ("EDM_VALUE" NOT NULL ENABLE)
 
  ALTER TABLE "EDM_WORK_REQUEST" MODIFY ("EDM_TYPE" NOT NULL ENABLE)
