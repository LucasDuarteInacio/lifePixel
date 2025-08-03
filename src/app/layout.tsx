import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "VidaPixel - Simulador de Vida",
  description: "Um jogo de simulação de vida estilo BitLife",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="pt-BR">
      <body className="antialiased">
        {children}
      </body>
    </html>
  );
}
