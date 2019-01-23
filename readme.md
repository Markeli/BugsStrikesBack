# Unit tests: Bugs strikes back

[![Build Status](https://travis-ci.org/Markeli/BugsStrikesBack.svg?branch=master)](https://travis-ci.org/Markeli/BugsStrikesBack)

Материалы с митапа `SpbDotNet-39`

Представим, что наш заказчик - сама Империя. Нам поручено создать Звезду смерти. ТЗ нормального нет, есть легаси код старого, провалившегося проекта, заказчик за любую ошибку приходит в ярость, поэтому мы сделаем MVP Звезды Смерти. 

## Архитектура Звезды Смерти

[![ClassDiagram](./artifacts/classDiagramm.png))]([classDiagramm](./artifacts/classDiagramm.png))

Она крайне проста, состоит из реактора, генератора защитного поля и луча Смерти:

- `IReactor` и его реализация отвечают за питание всего корабля: периодически генератор восполняет запасы энергии (`Energy`)
- `IProtectiveField` и его реализация создают вокруг Звезды Смерти защитное поле, которое сокращает урон от вражеского оружия (`Damage`). Для работы требуется реактор.
- `ICannon` и его реализация `SuperLaserCannon` - оружие Звезды Смерти, также для работы требует энергии от генератора

Если что-то приводит к фатальному сбою в Звезде Смерти, бросается `LastSeenException`.

## Структура репозитория

- `artifacts`: содержит диаграмму классов `DeathStart`
- `docs`: презентация с митапа
- `src`: основной код Звезды Смерти
- `tests`: все тесты и демо с митапа
  - `DeathStar.DummyTest`: примитивный тест с использованием консоли
  - `DeathStar.UnitTests`: `unit тесты` с использованием [xUnit](https://github.com/xunit/xunit) и [Moq](https://github.com/moq/moq4)
  - `DeathStar.UnitTestsWithFakes`: пример использования `MS Fakes`. Требуется `VS Enterpries`

## Требования

### Ветка `master`

- любая поддерживающая `.net core` OS
- `.Net Core 2.1`

### Ветка `ms_fakes`

- `Windows 7` или новее
- `.Net Framework 4.7.2`
- `VS Enterpise 2015` или новее

## Запуск

Сбилдить проект и запустить тесты для ветки `master` можно из `IDE` или через командую строку:

- `Linux`: `./build.sh`
- `Windows`: `.\build.ps1`

Можно запустить отдельно только сборку (запустить скрипт с ключом `-t Build`) или только тесты (`-t Tests`)