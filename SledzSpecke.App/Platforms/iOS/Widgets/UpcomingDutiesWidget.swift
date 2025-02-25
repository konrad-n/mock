import WidgetKit
import SwiftUI
import Intents

struct Provider: TimelineProvider {
    func placeholder(in context: Context) -> DutyEntry {
        DutyEntry(date: Date(), duties: [DutyInfo(title: "Dyżur na SOR", location: "Szpital Centralny", date: Date())])
    }

    func getSnapshot(in context: Context, completion: @escaping (DutyEntry) -> ()) {
        let entry = DutyEntry(date: Date(), duties: [DutyInfo(title: "Dyżur na SOR", location: "Szpital Centralny", date: Date())])
        completion(entry)
    }

    func getTimeline(in context: Context, completion: @escaping (Timeline<Entry>) -> ()) {
        // Implementacja pobierania dyżurów z aplikacji
        let sharedDefaults = UserDefaults(suiteName: "group.com.yourcompany.sledzspecke")
        var entries: [DutyEntry] = []
        
        // Aktualizuj co godzinę
        let currentDate = Date()
        for hourOffset in 0 ..< 24 {
            let entryDate = Calendar.current.date(byAdding: .hour, value: hourOffset, to: currentDate)!
            
            if let dutiesData = sharedDefaults?.data(forKey: "upcomingDuties"),
               let duties = try? JSONDecoder().decode([DutyInfo].self, from: dutiesData) {
                let entry = DutyEntry(date: entryDate, duties: duties)
                entries.append(entry)
            }
        }
        
        let timeline = Timeline(entries: entries, policy: .atEnd)
        completion(timeline)
    }
}

struct DutyInfo: Codable, Identifiable {
    var id = UUID()
    let title: String
    let location: String
    let date: Date
}

struct DutyEntry: TimelineEntry {
    let date: Date
    let duties: [DutyInfo]
}

struct UpcomingDutiesWidgetEntryView : View {
    var entry: Provider.Entry

    var body: some View {
        VStack(alignment: .leading) {
            Text("Najbliższe dyżury")
                .font(.headline)
                .padding(.bottom, 5)
            
            if entry.duties.isEmpty {
                Text("Brak nadchodzących dyżurów")
                    .font(.caption)
            } else {
                ForEach(entry.duties.prefix(3)) { duty in
                    HStack {
                        Circle()
                            .fill(Color.blue)
                            .frame(width: 8, height: 8)
                        VStack(alignment: .leading) {
                            Text(duty.title)
                                .font(.caption)
                                .fontWeight(.semibold)
                            Text(duty.location)
                                .font(.caption2)
                            Text(duty.date, style: .date)
                                .font(.caption2)
                                .foregroundColor(.secondary)
                        }
                    }
                    .padding(.vertical, 2)
                }
            }
        }
        .padding()
    }
}

@main
struct UpcomingDutiesWidget: Widget {
    let kind: String = "UpcomingDutiesWidget"

    var body: some WidgetConfiguration {
        StaticConfiguration(kind: kind, provider: Provider()) { entry in
            UpcomingDutiesWidgetEntryView(entry: entry)
        }
        .configurationDisplayName("Najbliższe dyżury")
        .description("Pokazuje listę nadchodzących dyżurów.")
        .supportedFamilies([.systemSmall, .systemMedium])
    }
}
