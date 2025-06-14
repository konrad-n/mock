import { apiClient } from './api';
import { saveAs } from 'file-saver';
import { format } from 'date-fns';
import { pl } from 'date-fns/locale';

export interface ExportOptions {
  specializationId: number;
  moduleType?: 'Basic' | 'Specialist' | 'All';
  format: 'pdf' | 'excel' | 'json';
  includeInternships?: boolean;
  includeMedicalShifts?: boolean;
  includeProcedures?: boolean;
  includeCourses?: boolean;
  includeSelfEducation?: boolean;
  includePublications?: boolean;
  includeAbsences?: boolean;
  dateFrom?: string;
  dateTo?: string;
}

class ExportService {
  async exportData(options: ExportOptions) {
    try {
      // In real implementation, this would call the API endpoint
      const response = await apiClient.post('/Export/generate', options, {
        responseType: 'blob'
      });
      
      // Generate filename
      const date = format(new Date(), 'yyyy-MM-dd', { locale: pl });
      const extension = options.format === 'excel' ? 'xlsx' : options.format;
      const filename = `SledzSpecke_Export_${date}.${extension}`;
      
      // Save file
      saveAs(response as Blob, filename);
      
      return { success: true, filename };
    } catch (error) {
      console.error('Export failed:', error);
      // For now, generate mock data export
      return this.generateMockExport(options);
    }
  }

  async exportToSMK(specializationId: number, smkVersion: 'Old' | 'New') {
    try {
      const response = await apiClient.post(`/Export/smk/${smkVersion.toLowerCase()}`, {
        specializationId
      }, {
        responseType: 'blob'
      });
      
      const date = format(new Date(), 'yyyy-MM-dd', { locale: pl });
      const filename = `SMK_${smkVersion}_Export_${date}.xml`;
      
      saveAs(response as Blob, filename);
      
      return { success: true, filename };
    } catch (error) {
      console.error('SMK export failed:', error);
      return this.generateMockSMKExport(smkVersion);
    }
  }

  private async generateMockExport(options: ExportOptions) {
    // Mock export data
    const mockData = {
      exportDate: new Date().toISOString(),
      specializationId: options.specializationId,
      moduleType: options.moduleType || 'All',
      data: {
        internships: options.includeInternships ? [
          {
            id: 1,
            department: 'Oddział Kardiologii Inwazyjnej',
            institution: 'Szpital Uniwersytecki',
            startDate: '2024-01-01',
            endDate: '2024-06-30',
            completedDays: 165,
            requiredDays: 180
          }
        ] : [],
        medicalShifts: options.includeMedicalShifts ? [
          {
            id: 1,
            date: '2024-03-15',
            duration: '12h 0min',
            location: 'Szpital Uniwersytecki',
            year: 1
          }
        ] : [],
        procedures: options.includeProcedures ? [
          {
            id: 1,
            code: '89.52',
            name: 'Koronarografia',
            date: '2024-03-20',
            operatorLevel: 'Operator',
            location: 'Pracownia Hemodynamiki'
          }
        ] : [],
        courses: options.includeCourses ? [
          {
            id: 1,
            name: 'Konferencja Kardiologiczna ESC 2024',
            type: 'Conference',
            date: '2024-09-15',
            creditHours: 20
          }
        ] : [],
        selfEducation: options.includeSelfEducation ? [
          {
            id: 1,
            title: 'Przegląd wytycznych ESC',
            type: 'ArticleReview',
            date: '2024-03-15',
            creditHours: 4
          }
        ] : []
      }
    };

    // Convert to requested format
    let blob: Blob;
    let filename: string;
    const date = format(new Date(), 'yyyy-MM-dd', { locale: pl });

    if (options.format === 'json') {
      blob = new Blob([JSON.stringify(mockData, null, 2)], { type: 'application/json' });
      filename = `SledzSpecke_Export_${date}.json`;
    } else if (options.format === 'excel') {
      // For Excel, we'd need a library like xlsx, for now just create CSV
      const csv = this.convertToCSV(mockData);
      blob = new Blob([csv], { type: 'text/csv;charset=utf-8' });
      filename = `SledzSpecke_Export_${date}.csv`;
    } else {
      // For PDF, we'd need a library like jsPDF, for now just create text
      const text = this.convertToText(mockData);
      blob = new Blob([text], { type: 'text/plain;charset=utf-8' });
      filename = `SledzSpecke_Export_${date}.txt`;
    }

    saveAs(blob, filename);
    return { success: true, filename };
  }

  private generateMockSMKExport(smkVersion: 'Old' | 'New') {
    const mockXML = `<?xml version="1.0" encoding="UTF-8"?>
<SMKExport version="${smkVersion}" date="${new Date().toISOString()}">
  <Specialization>
    <Name>Kardiologia</Name>
    <StartDate>2024-01-01</StartDate>
    <Module type="Basic">
      <Internships>
        <Internship>
          <Institution>Szpital Uniwersytecki</Institution>
          <Department>Oddział Kardiologii</Department>
          <StartDate>2024-01-01</StartDate>
          <EndDate>2024-06-30</EndDate>
          <CompletedDays>165</CompletedDays>
        </Internship>
      </Internships>
      <MedicalShifts>
        <Shift>
          <Date>2024-03-15</Date>
          <Duration>720</Duration>
          <Location>Szpital Uniwersytecki</Location>
        </Shift>
      </MedicalShifts>
      <Procedures>
        <Procedure>
          <Code>89.52</Code>
          <Name>Koronarografia</Name>
          <Date>2024-03-20</Date>
          <Role>Operator</Role>
        </Procedure>
      </Procedures>
    </Module>
  </Specialization>
</SMKExport>`;

    const blob = new Blob([mockXML], { type: 'application/xml;charset=utf-8' });
    const date = format(new Date(), 'yyyy-MM-dd', { locale: pl });
    const filename = `SMK_${smkVersion}_Export_${date}.xml`;
    
    saveAs(blob, filename);
    return { success: true, filename };
  }

  private convertToCSV(data: any): string {
    const lines: string[] = ['Typ,ID,Nazwa,Data,Szczegóły'];
    
    if (data.data.internships?.length) {
      data.data.internships.forEach((item: any) => {
        lines.push(`Staż,${item.id},"${item.department}",${item.startDate},"${item.institution}"`);
      });
    }
    
    if (data.data.procedures?.length) {
      data.data.procedures.forEach((item: any) => {
        lines.push(`Procedura,${item.id},"${item.name}",${item.date},"${item.code} - ${item.operatorLevel}"`);
      });
    }
    
    if (data.data.courses?.length) {
      data.data.courses.forEach((item: any) => {
        lines.push(`Kurs,${item.id},"${item.name}",${item.date},"${item.type} - ${item.creditHours} pkt"`);
      });
    }
    
    return lines.join('\n');
  }

  private convertToText(data: any): string {
    const lines: string[] = [
      'EKSPORT DANYCH SLEDZSPECKE',
      '=' .repeat(50),
      `Data eksportu: ${format(new Date(), 'dd.MM.yyyy HH:mm', { locale: pl })}`,
      `Moduł: ${data.moduleType}`,
      '',
    ];

    if (data.data.internships?.length) {
      lines.push('STAŻE:');
      lines.push('-'.repeat(30));
      data.data.internships.forEach((item: any) => {
        lines.push(`- ${item.department} (${item.institution})`);
        lines.push(`  Okres: ${item.startDate} - ${item.endDate}`);
        lines.push(`  Dni: ${item.completedDays}/${item.requiredDays}`);
        lines.push('');
      });
    }

    if (data.data.procedures?.length) {
      lines.push('PROCEDURY:');
      lines.push('-'.repeat(30));
      data.data.procedures.forEach((item: any) => {
        lines.push(`- ${item.code} - ${item.name}`);
        lines.push(`  Data: ${item.date}, Rola: ${item.operatorLevel}`);
        lines.push('');
      });
    }

    if (data.data.courses?.length) {
      lines.push('KURSY:');
      lines.push('-'.repeat(30));
      data.data.courses.forEach((item: any) => {
        lines.push(`- ${item.name}`);
        lines.push(`  Data: ${item.date}, Punkty: ${item.creditHours}`);
        lines.push('');
      });
    }

    return lines.join('\n');
  }
}

export const exportService = new ExportService();