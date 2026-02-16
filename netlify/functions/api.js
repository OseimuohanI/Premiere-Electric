const CHAT_RESPONSES = [
  {
    test: (m) => m.includes("service") || m.includes("what do you do"),
    reply:
      "We offer residential wiring, commercial electrical work, maintenance & repairs, and equipment installation. Would you like more details about any specific service?"
  },
  {
    test: (m) => m.includes("price") || m.includes("cost") || m.includes("quote"),
    reply:
      "Our pricing varies by service. For a detailed quote, please fill out our contact form or call us at (555) 123-4567. We offer free estimates!"
  },
  {
    test: (m) => m.includes("emergency") || m.includes("urgent"),
    reply:
      "For emergencies, call us directly at (555) 123-4567. We offer 24/7 emergency support for urgent electrical issues."
  },
  {
    test: (m) => m.includes("hours") || m.includes("open"),
    reply:
      "We're open Monday-Friday, 8AM-6PM. Emergency services are available 24/7. How can we help you?"
  },
  {
    test: (m) => m.includes("contact") || m.includes("reach"),
    reply:
      "You can reach us at (555) 123-4567 or email info@premierelectric.com. You can also fill out our contact form below!"
  },
  {
    test: (m) => m.includes("hello") || m.includes("hi"),
    reply: "Hello! How can I assist you with your electrical needs today?"
  },
  {
    test: (m) => m.includes("thank"),
    reply: "You're welcome! Feel free to reach out if you have any other questions."
  }
];

const CONTACT_FROM_EMAIL =
  process.env.CONTACT_FROM_EMAIL || "contact@premiereelectric.com";
const CONTACT_ADMIN_EMAIL =
  process.env.CONTACT_ADMIN_EMAIL || "osemekeme@gmail.com";
const RESEND_API_KEY = process.env.RESEND_API_KEY || "";
const SUPABASE_URL =
  process.env.SUPABASE_URL || "https://csihmcsbvybtmtzxunld.supabase.co";
const SUPABASE_KEY = process.env.SUPABASE_KEY || "";

function json(statusCode, body) {
  return {
    statusCode,
    headers: {
      "Content-Type": "application/json",
      "Access-Control-Allow-Origin": "*"
    },
    body: JSON.stringify(body)
  };
}

function options() {
  return {
    statusCode: 204,
    headers: {
      "Access-Control-Allow-Origin": "*",
      "Access-Control-Allow-Methods": "GET, POST, OPTIONS",
      "Access-Control-Allow-Headers": "Content-Type"
    }
  };
}

function getSessionId(provided) {
  if (provided) {
    return provided;
  }

  if (globalThis.crypto && typeof globalThis.crypto.randomUUID === "function") {
    return globalThis.crypto.randomUUID();
  }

  return `sess-${Date.now().toString(36)}`;
}

async function sendResendEmail({ to, subject, html }) {
  if (!RESEND_API_KEY) {
    return { ok: false, error: "RESEND_API_KEY is not set" };
  }

  const response = await fetch("https://api.resend.com/emails", {
    method: "POST",
    headers: {
      Authorization: `Bearer ${RESEND_API_KEY}`,
      "Content-Type": "application/json"
    },
    body: JSON.stringify({
      from: CONTACT_FROM_EMAIL,
      to,
      subject,
      html
    })
  });

  if (!response.ok) {
    const text = await response.text();
    return { ok: false, error: text };
  }

  return { ok: true };
}

async function insertSupabaseSubmission(payload) {
  if (!SUPABASE_URL || !SUPABASE_KEY) {
    return { ok: false, error: "SUPABASE_URL or SUPABASE_KEY missing" };
  }

  const response = await fetch(`${SUPABASE_URL}/rest/v1/contact_submissions`, {
    method: "POST",
    headers: {
      apikey: SUPABASE_KEY,
      Authorization: `Bearer ${SUPABASE_KEY}`,
      "Content-Type": "application/json",
      Prefer: "return=representation"
    },
    body: JSON.stringify([payload])
  });

  if (!response.ok) {
    const text = await response.text();
    return { ok: false, error: text };
  }

  const data = await response.json();
  return { ok: true, data };
}

function buildCustomerEmail(payload, ticketId) {
  return `
    <html>
      <body style="font-family: Arial, sans-serif; color: #333;">
        <div style="max-width: 600px; margin: 0 auto; padding: 20px;">
          <h2 style="color: #1a3a52;">Thank You for Contacting Premiere Electric</h2>
          <p>Dear ${payload.fullName},</p>
          <p>We have received your message and appreciate your interest in Premiere Electric. Our team will review your inquiry and get back to you shortly.</p>
          <div style="background: #f0f0f0; padding: 15px; border-radius: 5px; margin: 20px 0;">
            <strong>Your Ticket ID:</strong> ${ticketId}<br />
            <em>Please keep this ID for your records.</em>
          </div>
          <h3>Your Inquiry Details:</h3>
          <p><strong>Subject:</strong> ${payload.subject}</p>
          <p><strong>Preferred Contact:</strong> ${payload.preferredContact || "Any"}</p>
          <p>We typically respond within 24-48 business hours. If your request is urgent, please call us directly at (555) 123-4567.</p>
          <p>Best regards,<br />Premiere Electric Team</p>
        </div>
      </body>
    </html>
  `;
}

function buildAdminEmail(payload, ticketId) {
  return `
    <html>
      <body>
        <h2>New Contact Form Submission</h2>
        <p><strong>Ticket ID:</strong> ${ticketId}</p>
        <p><strong>Name:</strong> ${payload.fullName}</p>
        <p><strong>Email:</strong> ${payload.email}</p>
        <p><strong>Phone:</strong> ${payload.phoneNumber || "Not provided"}</p>
        <p><strong>Subject:</strong> ${payload.subject}</p>
        <p><strong>Service Category:</strong> ${payload.serviceCategory || "Not specified"}</p>
        <p><strong>Preferred Contact:</strong> ${payload.preferredContact || "Not specified"}</p>
        <h3>Message:</h3>
        <p>${payload.message}</p>
        <p><em>Submitted at: ${new Date().toISOString()}</em></p>
      </body>
    </html>
  `;
}

async function handleContactSubmit(payload) {
  const {
    fullName,
    email,
    phoneNumber,
    subject,
    message,
    serviceCategory,
    preferredContact
  } = payload;

  if (!fullName || !email || !subject || !message) {
    return json(400, { success: false, message: "Missing required fields." });
  }

  const ticketId = `PE-${Date.now().toString(36)}-${Math.random()
    .toString(36)
    .slice(2, 8)}`;

  const submissionPayload = {
    full_name: fullName,
    email,
    phone_number: phoneNumber || null,
    subject,
    message,
    service_category: serviceCategory || null,
    preferred_contact: preferredContact || null,
    ticket_id: ticketId,
    submitted_at: new Date().toISOString()
  };

  const storageResult = await insertSupabaseSubmission(submissionPayload);
  if (!storageResult.ok) {
    return json(500, {
      success: false,
      message: "Storage not configured or failed.",
      error: storageResult.error
    });
  }

  const customerEmail = buildCustomerEmail(payload, ticketId);
  const adminEmail = buildAdminEmail(payload, ticketId);

  const customerResult = await sendResendEmail({
    to: email,
    subject: "We Received Your Message - Premiere Electric",
    html: customerEmail
  });

  if (!customerResult.ok) {
    return json(500, {
      success: false,
      message: "Email delivery failed.",
      error: customerResult.error
    });
  }

  const adminResult = await sendResendEmail({
    to: CONTACT_ADMIN_EMAIL,
    subject: `New Contact Form Submission - ${subject}`,
    html: adminEmail
  });

  if (!adminResult.ok) {
    return json(500, {
      success: false,
      message: "Admin notification failed.",
      error: adminResult.error
    });
  }

  return json(200, {
    success: true,
    message: "Thanks! Your message has been received.",
    ticketId
  });
}

function handleChatMessage(payload) {
  const userMessage = String(payload.message || "");
  const message = userMessage.toLowerCase();
  const match = CHAT_RESPONSES.find((entry) => entry.test(message));
  const botResponse = match
    ? match.reply
    : "I'd be happy to help! You can ask me about our services, pricing, hours, or how to contact us. What would you like to know?";

  const sessionId = getSessionId(payload.sessionId);

  return json(200, {
    success: true,
    sessionId,
    botResponse
  });
}

exports.handler = async (event) => {
  if (event.httpMethod === "OPTIONS") {
    return options();
  }

  const path = event.path || "";
  let payload = {};

  if (event.body) {
    try {
      payload = JSON.parse(event.body);
    } catch (error) {
      return json(400, { success: false, message: "Invalid JSON body." });
    }
  }

  if (event.httpMethod === "POST" && path.endsWith("/api/chat/message")) {
    return handleChatMessage(payload);
  }

  if (event.httpMethod === "POST" && path.endsWith("/api/contact/submit")) {
    return handleContactSubmit(payload);
  }

  return json(404, { success: false, message: "Not found." });
};