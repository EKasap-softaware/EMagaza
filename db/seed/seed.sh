#!/bin/bash
# /seed altındaki her <koleksiyon>.json dosyasını aynı adlı MongoDB koleksiyonuna yükler.
# Koleksiyon boş değilse dokunmaz (tekrar çalıştırmak güvenli).
set -e

HOST="${MONGO_HOST:-mongo}"
DB="${MONGO_DB:-emagaza}"

until mongosh --host "$HOST" --quiet --eval "db.runCommand({ ping: 1 }).ok" > /dev/null 2>&1; do
  echo "MongoDB bekleniyor..."
  sleep 1
done

for file in /seed/*.json; do
  collection=$(basename "$file" .json)
  count=$(mongosh --host "$HOST" --quiet --eval "db.getSiblingDB('$DB').getCollection('$collection').countDocuments()")

  if [ "$count" = "0" ]; then
    mongoimport --host "$HOST" --db "$DB" --collection "$collection" --file "$file" --jsonArray
  else
    echo "$collection dolu ($count kayıt), atlandı."
  fi
done
